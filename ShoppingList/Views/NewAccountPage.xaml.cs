using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using ShoppingList.Models;
    
namespace ShoppingList.Views;

public partial class NewAccountPage : ContentPage
{
    public NewAccountPage()
    {
        InitializeComponent();
        Title = "Create New Account";
    }

    async void CreateAccount_OnClicked(object sender, EventArgs e)
    {
        //validate that passwords match
        if (txtPassword1.Text != txtPassword2.Text)
        {
            await DisplayAlert("Error", "Passwords don't match", "OK");
            return;
        }
        //is email valid aka = @ .
        if (!string.IsNullOrEmpty(txtEmail.Text) && txtEmail.Text.Contains('@') && txtEmail.Text.Contains('.'))
        {
            int aIndex = txtEmail.Text.IndexOf('@');
            int dotIndex = txtEmail.Text.LastIndexOf('.');

            if (aIndex > dotIndex)
            {
                await DisplayAlert("Error", "the email address you've entered is invalid", "ok");
                return;
            }
        }
            //api stuff
            var data = JsonConvert.SerializeObject(new UserAccount(txtUser.Text, txtPassword1.Text, txtEmail.Text));
            
            var client = new HttpClient();
            var response = await client.PostAsync(new Uri("https://joewetzel.com/fvtc/account/createuser"),
                        new StringContent(data, Encoding.UTF8, "application/json"));
            
            var AccountStatus = response.Content.ReadAsStringAsync().Result;
            
            //does user exist 
            if (AccountStatus == "user exists")
            {
                await DisplayAlert("Error", "User has already been taken!", "ok");
                return;
            }
            
            //does email exist 
            if (AccountStatus == "email exists")
            {
                await DisplayAlert("Error", "email has already in use!", "ok");
                return;
            }

            if (AccountStatus =="complete")
            {
                response = await client.PostAsync(new Uri("https://joewetzel.com/fvtc/account/login"),
                    new StringContent(data, Encoding.UTF8, "application/json"));
            
                var SKey = response.Content.ReadAsStringAsync().Result;

                if (!string.IsNullOrEmpty(SKey) || SKey.Length > 50 )
                {
                    App.SessionKey = SKey;
                    Navigation.PopModalAsync();
                }
                else
                {
                    await DisplayAlert("Error", "Sorry, could not login!", "ok");
                    return;
                }
            }
            else
            {
                await DisplayAlert("Error", "Sorry, we had an issue creating your account!", "ok");
                return;
            }
    }
}