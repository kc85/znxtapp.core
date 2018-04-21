var success_code = 1;

function validateEmail($email) {
    if ($email.length == 0) {
        return false;
    }
    var emailReg = /^([\w-\.]+@([\w-]+\.)+[\w-]{2,4})?$/;
    return emailReg.test($email);
}

function validatePhone(field) {
    if (field.length == 0) {
        return false;
    }
    if (field.match(/^\d{10}/)) {
        return true;
    }
  
    return false;
}