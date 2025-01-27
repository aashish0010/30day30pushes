﻿using Entity.Model;
using Infrastructure.Dapper;
using Infrastructure.Jwt;
using Microsoft.Extensions.Configuration;
using Service.Interface;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Service.Service
{
    public class LoginService : ILogininterface
    {
        private readonly IConfiguration _configuration;
        public LoginService(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        Dictionary<string, string> dict = new Dictionary<string, string>();
        public async Task<dynamic> Login(LoginRequest request)
        {
            var token = new TokenService(_configuration);
            var login = new Login();
            string sql = "sp_user";
            dict.Add("@nameorcitizen", request.UserName);
            dict.Add("@password", request.Password);
            dict.Add("@flag", "Login");
            var res = await DbHelper.RunQueryWithoutModelAsync(sql, dict);

            if(res.FirstOrDefault().StatusCode==400)
            {
                login.StatusCode = res.FirstOrDefault().StatusCode;
                login.Message = res.FirstOrDefault().Message;
            }
            else
            {
                login.Username = res.FirstOrDefault().username;
                login.Email = res.FirstOrDefault().email;
                login.Citizenshipnumber = res.FirstOrDefault().citizenshipnumber;
                login.Designationname = res.FirstOrDefault().designationname;
                login.Token = token.TokenGenerateString(login.Username);
            }
            return login;
        }

        public async Task<IEnumerable<GetByIdLogin>> GetUserById(string username)
        {
            string sql = "sp_user";
            Dictionary<string,string> dict = new Dictionary<string, string>();
            dict.Add("@username", username);
            dict.Add("@flag", "GetById");
            var restr =await DbHelper.RunQueryWithModelAync<GetByIdLogin>(sql, dict);
            return restr;
        }

        public async Task<IEnumerable<CommonResponse>> Register(Register register)
        {
            var sql = "sp_user";
            Dictionary<string, string> dict = new Dictionary<string, string>();
            dict.Add("@username", register.username);
            dict.Add("@password", register.password);
            dict.Add("@email", register.email);
            dict.Add("@firstname", register.firstname);
            dict.Add("@middlename", register.middlename);
            dict.Add("@lastname", register.lastname);
            dict.Add("@citizenshipnumber", register.citizenshipnumber);
            dict.Add("@iscitizen", "");
            dict.Add("@phonenumber", register.phonenumber);
            dict.Add("@flag", "Register");
            var data =await DbHelper.RunQueryWithModelAync<CommonResponse>(sql, dict);
            return data;
        }
    }
}
