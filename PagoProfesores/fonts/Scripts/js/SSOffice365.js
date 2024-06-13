$(function () {

    $(window).load(function () {

        var url = window.location.href
        var arr = url.split("/");
        var domain = arr[0] + "//" + arr[2]

        if (window.location.hash) {

            var hash = window.location.hash.substring(1); //Puts hash in variable, and removes the # character
            window.location.href = domain + "/SSOffice365/VerifyToken/?" + hash;

            


        } else {
            
            signIn();
        }
     

    });


});



  var ADAL = new AuthenticationContext({
      instance: 'https://login.microsoftonline.com/',
      tenant: 'common', //COMMON OR YOUR TENANT ID

       // clientId: '09fa3fa2-106e-41b2-91f4-75026bdaa9ee', //REPLACE WITH YOUR CLIENT ID TEST
      clientId: '5b542d15-ce2c-4adc-8aa8-7d8e4572e0c8', //REPLACE WITH YOUR CLIENT ID PRODUCCION
      // redirectUri: 'http://localhost:58402/ssoOffice365.html',// REPLACE WITH YOUR REDIRECT URL
      // redirectUri: 'https://40.84.224.118/ssoOffice365.html',
       redirectUri: 'https://pagoprofesores.redanahuac.mx/ssoOffice365.html',

      callback: userSignedIn,
      popUp: false
  });


function signIn() {
    ADAL.login();
}

function userSignedIn(err, token) {
    console.log('userSignedIn called');
    if (!err) {
        console.log("token: " + token);
        showWelcomeMessage();
    }
    else {
        console.error("error: " + err);
    }
}

function showWelcomeMessage() {
    var user = ADAL.getCachedUser();
    var divWelcome = document.getElementById('WelcomeMessage');
    divWelcome.innerHTML = "Welcome " + user.profile.name;
}

