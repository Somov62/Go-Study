using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace Go_Study_Mobile.Services
{
    internal class StartProgramService
    {
        public StartProgramService()
        {

        }

        public void Start()
        {
            AuthService authService = new AuthService();

            if (!authService.IsAuthorized)
            {

            }
        }
    }
}
