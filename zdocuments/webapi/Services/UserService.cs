using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using webapi.Entities;
using webapi.Helpers;

namespace webapi.Services
{
    public interface IUserService
    {
        User Authenticate(string username, string password) ;
        IEnumerable<User> GetAll();
        User GetByID(int id);
        User Create(User user, string password);
       
        User ValidateOTP(User user);

        User FindUser(User user);

        void Update(User user, string password = null);
        void Delete(int id);
    }

    public class UserService : IUserService
    {
        private DataContext _context;
        public UserService(DataContext context)
        {
            _context = context;
        }

        public User Authenticate(string phone, string password)
        {
            if (string.IsNullOrEmpty(phone) || string.IsNullOrEmpty(password))
                return null;

            var user = _context.Users.SingleOrDefault(x => x.Phone == phone);
            
            //check if the username exists
            if(user == null)
                throw new AppException("Phone number not registered");

            //check if the user has not completed the registration yet
            if(user.OTP != 0)
                throw new AppException("Registration pending");

            //check if the password is correct
            if(!VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt))
                throw new AppException("Invalid phone number or password");

            //authentication successful
            return user;
        }

        public IEnumerable<User> GetAll()
        {
            return _context.Users;
        }

        public User GetByID(int id)
        {
            return _context.Users.Find(id);
        }

        public User Create(User user, string password)
        {
            //validation
            if (string.IsNullOrWhiteSpace(password))
                throw new AppException("Password is required");
        
            if(_context.Users.Any(x=> x.Phone == user.Phone && x.OTP == 0))
                throw new AppException("Phone number :"+user.Phone+" is already taken");

            byte[] passwordHash, passwordSalt;
            CreatePasswordHash(password, out passwordHash, out passwordSalt);                

            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;

            var euser = _context.Users.SingleOrDefault(x => x.Phone == user.Phone && x.OTP != 0);

            //if(_context.Users.Any(x=> x.Phone == user.Phone && x.OTP != 0)) 
            if(euser != null)//Previous registration pending
            {
                euser.FirstName = user.FirstName;
                euser.LastName = user.LastName;
                euser.UserType = user.UserType;
                euser.Email = user.Email;
                euser.OTP = user.OTP;
                euser.PasswordHash = user.PasswordHash;
                euser.PasswordSalt = user.PasswordSalt;
                _context.Entry(euser).State = EntityState.Modified;
            }
            else
                _context.Users.Add(user);
            _context.SaveChanges();

            return user;
        }

        public User ValidateOTP(User user)
        {
            var euser = _context.Users.SingleOrDefault(x => x.Phone == user.Phone && x.OTP == user.OTP && x.OTP != 0);

            if(euser != null) 
            {
                euser.OTP = 0;
                _context.Entry(euser).State = EntityState.Modified;
                _context.SaveChanges();
            }
            else
                throw new AppException("Invalid OTP");
            
            return user;
        }

        public User FindUser(User userParam)
        {
            var user = _context.Users.FromSql("SELECT * FROM dbo.Users WHERE Phone = @Phone AND Email = @Email", new SqlParameter("@Phone", userParam.Phone), 
                                       new SqlParameter("@Email", userParam.Email)).FirstOrDefault();

            //user does not exist
            if(user == null)
                throw new AppException("User not found");

            string query =  "MERGE dbo.tbl_phone_validation src ";
            query = query + "USING (SELECT @Phone AS Phone, @Otp AS Otp) dest ";
		    query = query +    "ON src.Phone = dest.Phone ";
		    query = query +  "WHEN MATCHED THEN ";
	        query = query +"UPDATE SET src.Otp = dest.Otp ";
	        query = query +  "WHEN NOT MATCHED BY TARGET THEN ";
	        query = query +"INSERT (Phone, Otp) ";
	        query = query +"VALUES (dest.Phone, dest.Otp);";
            _context.Database.ExecuteSqlCommand(query, new SqlParameter("@Phone", userParam.Phone), 
                                       new SqlParameter("@Otp", userParam.OTP.ToString()));

            return user;
        }

        public void Update(User userParam, string password = null)
        {
            var outParam = new SqlParameter();
            outParam.ParameterName = "Output";
            outParam.SqlDbType = SqlDbType.Bit;
            outParam.Direction = ParameterDirection.Output;
            string json = "{\"Phone\":\""+userParam.Phone+"\",\"Otp\":\""+userParam.OTP+"\"}";

            var opt = _context.Database.ExecuteSqlCommand("EXEC dbo.usp_verify_phone_otp @json, @output OUT", new SqlParameter("@json", json), outParam);
            if(!(bool)outParam.Value)
                throw new AppException("Invalid OTP");   

            var user = _context.Users.FromSql("SELECT * FROM dbo.Users WHERE Phone = @Phone AND Email = @Email", new SqlParameter("@Phone", userParam.Phone), 
                                       new SqlParameter("@Email", userParam.Email)).FirstOrDefault();
            //user does not exist
            if(user == null)
                throw new AppException("User not found");

            //update user properties
            //user.FirstName = userParam.FirstName == null?user.FirstName:userParam.FirstName;
            //user.LastName = userParam.LastName == null?user.LastName:userParam.LastName;

            //update password if it was entered
            if(!string.IsNullOrWhiteSpace(password))
            {
                byte[] passwordHash, passwordSalt;
                CreatePasswordHash(password, out passwordHash, out passwordSalt);

                user.PasswordHash = passwordHash;
                user.PasswordSalt = passwordSalt;
            }    

            _context.Users.Update(user);
            _context.SaveChanges();
        }

        public void Delete(int id)
        {
            var user = _context.Users.Find(id);
            if(user != null)
            {
                _context.Users.Remove(user);
                _context.SaveChanges();
            }
        }

        //private helper method
        private static void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            if(password == null) throw new ArgumentNullException("password");    
            if(string.IsNullOrWhiteSpace(password)) throw new ArgumentException("Value cannot be empty or whitespace only string.", "password");

            using(var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        private static bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            if(password == null) throw new ArgumentNullException("password");    
            if(string.IsNullOrWhiteSpace(password)) throw new ArgumentException("Value cannot be empty or whitespace only string.", "password");

            if(passwordHash.Length != 64) throw new ArgumentException("Invalid length of password hash (64 bytes expected).", "passwordHash");
            if(passwordSalt.Length != 128) throw new ArgumentException("Invalid length of password salt (128 bytes expected).", "passwordHash");

            using(var hmac = new System.Security.Cryptography.HMACSHA512(passwordSalt))
            {
                var computeHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                for(int i = 0; i < computeHash.Length; i++)
                {
                    if(computeHash[i] != passwordHash[i])
                        return false;
                }
                return true;
            }
        }
    }
}