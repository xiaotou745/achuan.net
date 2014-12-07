//通用验证方法
//CreatTime:2012-09-21
//EditRecord:[No.][Editor][EditTime][Remark]
// 1.
// 2.
// 3.


//是否为空
function IsEmpty(str) {
    if (str == null || str == "" || $.trim(str)=="") {
        return true;
    }
    return false;
}

//邮箱验证
function IsMail(mail) {
    return (new RegExp(/^\w+((-\w+)|(\.\w+))*\@[A-Za-z0-9]+((\.|-)[A-Za-z0-9]+)*\.[A-Za-z0-9]+$/).test(mail));
}

//验证版本号
function IsVersion(version) {
    return (new RegExp(/^\d{1,8}\.\d{1}\.\d{1}\.\d{1}$/).test(version));
}

//验证电话号码 正确格式为：XXXX-XXXXXXX-XXXX
function IsTelephone(phone) {
    return (new RegExp(/^(\(\d{3,4}\)|\d{3,4}-)?\d{7,8}(-\d{1,4})?$/).test(phone));
}
//验证手机号
function IsMobilePhone(phone) {
    if (!(/^1[3-8]\d{9}$/.test(phone))) {
        return false;
    }
    return true;
}

//验证非零的正整数
function IsUint(num) {
    return (new RegExp(/^\+?[1-9][0-9]*$/).test(num));
}

//验证有1-3位小数的正实数
function IsDecimal(num) {
    return (new RegExp(/^[0-9]+(.[0-9]{1,3})?$/).test(num));
}

//验证大于等于0的整数
function IsInt(num) {
    return /^\d+$/.test(num);
}
