//获取页面url
function getQueryString(name) {
    var reg = new RegExp("(^|&)" + name + "=([^&]*)(&|$)", "i");
    var r = window.location.search.substr(1).match(reg);
    if (r != null) return unescape(r[2]); return '';
}

function GUID() {
    this.date = new Date();   /* 判断是否初始化过，如果初始化过以下代码，则以下代码将不再执行，实际中只执行一次 */
    if (typeof this.newGUID != 'function') {   /* 生成GUID码 */
        GUID.prototype.newGUID = function () {
            this.date = new Date(); var guidStr = '';
            sexadecimalDate = this.hexadecimal(this.getGUIDDate(), 16);
            sexadecimalTime = this.hexadecimal(this.getGUIDTime(), 16);
            for (var i = 0; i < 9; i++) {
                guidStr += Math.floor(Math.random() * 16).toString(16);
            }
            guidStr += sexadecimalDate;
            guidStr += sexadecimalTime;
            while (guidStr.length < 32) {
                guidStr += Math.floor(Math.random() * 16).toString(16);
            }
            return this.formatGUID(guidStr);
        }
        /* * 功能：获取当前日期的GUID格式，即8位数的日期：19700101 * 返回值：返回GUID日期格式的字条串 */
        GUID.prototype.getGUIDDate = function () {
            return this.date.getFullYear() + this.addZero(this.date.getMonth() + 1) + this.addZero(this.date.getDay());
        }
        /* * 功能：获取当前时间的GUID格式，即8位数的时间，包括毫秒，毫秒为2位数：12300933 * 返回值：返回GUID日期格式的字条串 */
        GUID.prototype.getGUIDTime = function () {
            return this.addZero(this.date.getHours()) + this.addZero(this.date.getMinutes()) + this.addZero(this.date.getSeconds()) + this.addZero(parseInt(this.date.getMilliseconds() / 10));
        }
        /* * 功能: 为一位数的正整数前面添加0，如果是可以转成非NaN数字的字符串也可以实现 * 参数: 参数表示准备再前面添加0的数字或可以转换成数字的字符串 * 返回值: 如果符合条件，返回添加0后的字条串类型，否则返回自身的字符串 */
        GUID.prototype.addZero = function (num) {
            if (Number(num).toString() != 'NaN' && num >= 0 && num < 10) {
                return '0' + Math.floor(num);
            } else {
                return num.toString();
            }
        }
        /*  * 功能：将y进制的数值，转换为x进制的数值 * 参数：第1个参数表示欲转换的数值；第2个参数表示欲转换的进制；第3个参数可选，表示当前的进制数，如不写则为10 * 返回值：返回转换后的字符串 */GUID.prototype.hexadecimal = function (num, x, y) {
            if (y != undefined) { return parseInt(num.toString(), y).toString(x); }
            else { return parseInt(num.toString()).toString(x); }
        }
        /* * 功能：格式化32位的字符串为GUID模式的字符串 * 参数：第1个参数表示32位的字符串 * 返回值：标准GUID格式的字符串 */
        GUID.prototype.formatGUID = function (guidStr) {
            var str1 = guidStr.slice(0, 8) + '-', str2 = guidStr.slice(8, 12) + '-', str3 = guidStr.slice(12, 16) + '-', str4 = guidStr.slice(16, 20) + '-', str5 = guidStr.slice(20);
            return str1 + str2 + str3 + str4 + str5;
        }
    }
}


//随机数
function GetRandomNum(Min, Max) {
    var Range = Max - Min;
    var Rand = Math.random();
    return (Min + Math.round(Rand * Range));
}

/*js键值对*/
function Map() {
    this.keys = new Array();
    this.data = new Array();
    //添加键值对
    this.set = function (key, value) {
        if (this.data[key] == null) {//如键不存在则身【键】数组添加键名
            this.keys.push(value);
        }
        this.data[key] = value;//给键赋值
    };
    //获取键对应的值
    this.get = function (key) {
        return this.data[key];
    };
    //去除键值，(去除键数据中的键名及对应的值)
    this.remove = function (key) {
        this.keys.remove(key);
        this.data[key] = null;
    };
    //判断键值元素是否为空
    this.isEmpty = function () {
        return this.keys.length == 0;
    };
    //获取键值元素大小
    this.size = function () {
        return this.keys.length;
    };
}

//字符串长度
String.prototype.length = function () {
    var len = 0;
    for (var i = 0; i < this.length; i++) {
        if (this.charCodeAt(i) > 127 || this.charCodeAt(i) == 94) {
            len += 2;
        } else {
            len++;
        }
    }
    return len;
}


Array.prototype.contains = function (element) {
    for (var i = 0; i < this.length; i++) {
        if (this[i] == element) {
            return true;
        }
    }
    return false;
}
Array.prototype.indexOf = function (val) {
    for (var i = 0; i < this.length; i++) {
        if (this[i] == val) return i;
    }
    return -1;
};
Array.prototype.remove = function (val) {
    var index = this.indexOf(val);
    if (index > -1) {
        this.splice(index, 1);
    }
};

function replaceAll(str, str_old, str_new) {
    str = str.replace(str_old, str_new);
    if (str.indexOf(str_old) != -1) {
        return arguments.callee(str, str_old, str_new)
    } else {
        return str;
    }
}

/*-------通用方法------*/
/*使用前请保证在同级页面中加入了对Zebra_Dialog的引用*/
/*input类型标签的输入检测(不含checkbox)*/
/*domName:该dom节点的名称*/
/*strAlert:错误提示*/
/*dom节点的数据长度(-1表示不限制数据长度)*/
function CheckInput(domName, strAlert, numlength) {
    var pattern = new RegExp("[~'!@#$%^&*()-+_=:]"); //检验非法字符
    var strValue = $.trim($('input[name=' + domName + ']').val());
    if (strValue == "") {
        showMsg('' + strAlert + '不能为空 ！');
        $('input[name=' + domName + ']').focus();
        return false;
    } else {
        if (pattern.test(strValue)) {
            showMsg('' + strAlert + '中含有非法字符，请检查 ！');
            $('input[name=' + domName + ']').focus();
            return false;
        } else {
            if (parseInt(numlength) != -1) {
                if (strValue.length < parseInt(numlength)) {
                    showMsg('' + strAlert + '的长度至少为' + numlength + '位，请检查您的输入！');
                    $('input[name=' + domName + ']').focus();
                    return false;
                } else {
                    return true;
                }
            } else {
                return true;
            }
        }
    }
};



/*select类型标签的输入检测*/
/*domName:该dom节点的名称*/
/*strAlert:错误提示*/
function CheckSelect(domName, strAlert) {
    var strValue = $('select[name=' + domName + ']').val();
    if (strValue == "") {
        showMsg('' + strAlert + '不能为空 ！');
        $('select[name=' + domName + ']').focus();
        return false;
    } else {
        return true;
    }

};

/*检验数字整形*/
/*num:接受检验的字符*/
/*strAlert:错误提示*/
/*numMin:数值最小范围(-1表示不限制最小值)*/
/*numMax:数值最大范围(-1表示不限制最大值)*/
function CheckNumInt(num, strAlert, numMin, numMax) {
    if ($.trim(num) == "") {
        showMsg('' + strAlert + '不能为空 ！');
        return false;
    }
    if (isNaN(num)) {
        showMsg('' + strAlert + '需要输入数字类型 ！');
        return false;
    } else {
        if (parseInt(numMin) != -1) {
            if (parseInt(num) < parseInt(numMin)) {
                showMsg('' + strAlert + '的数值不能小于' + numMin + ' ！');
                return false;
            }
        }

        if (parseInt(numMax) != -1) {
            if (parseInt(num) > parseInt(numMax)) {
                showMsg('' + strAlert + '的数值不能大于' + numMax + ' ！');
                return false;
            }
        }
        return true;
    }
};


function CheckNumDecimal(value, strAlert, numlength) {
    if ($.trim(value) == "") {
        showMsg('' + strAlert + '不能为空 ！');
        return false;
    }
    if (isNaN(value)) {
        showMsg('' + strAlert + '需要输入小数类型 ！');
        return false;
    }

    if (value != null && value != '') {
        var decimalIndex = value.indexOf('.');
        if (decimalIndex == '-1') {
            return true;
        } else {
            var decimalPart = value.substring(decimalIndex + 1);
            if (decimalPart.length > parseInt(numlength)) {
                showMsg('' + strAlert + '最多可以输入位' + numlength + '小数 ！');
                return false;
            } else {
                return true;
            }
        }
    }
    return false;

};



//验证11位手机号 1开头
function CheckMobile(str) {
    var re = /^1\d{10}$/
    if (re.test(str)) {
        return true;
    } else {
        return false;
    }
};

/*验证电话号码
验证规则：区号+号码，区号以0开头，3位或4位
号码由7位或8位数字组成
区号与号码之间可以无连接符，也可以“-”连接
如01088888888,010-88888888,0955-7777777 */
function CheckPhone(str) {
    var re = /^0\d{2,3}-?\d{7,8}$/;
    if (re.test(str)) {
        return true;
    } else {
        return false;
    }
};

function CheckEmail(str) {
    var re = /^(\w-*\.*)+@(\w-?)+(\.\w{2,})+$/
    if (re.test(str)) {
        return true;
    } else {
        showMsg(' 邮箱号格式不正确');
        return false;
    }
};

//检测身份证号 18和15
function checkIdcard(num) {

    num = num.toLocaleUpperCase();

    //身份证号码为15位或者18位，15位时全为数字，18位前17位为数字，最后一位是校验位，可能为数字或字符X。
    if (!(/(^\d{15}$)|(^\d{17}([0-9]|X)$)/.test(num))) {
        return false;
    }
    //校验位按照ISO 7064:1983.MOD 11-2的规定生成，X可以认为是数字10。
    //下面分别分析出生日期和校验位
    var len, re;
    len = num.length;
    if (len == 15) {
        re = new RegExp(/^(\d{6})(\d{2})(\d{2})(\d{2})(\d{3})$/);
        var arrSplit = num.match(re);

        //检查生日日期是否正确
        var dtmBirth = new Date('19' + arrSplit[2] + '/' + arrSplit[3] + '/' + arrSplit[4]);
        var bGoodDay;
        bGoodDay = (dtmBirth.getYear() == Number(arrSplit[2])) && ((dtmBirth.getMonth() + 1) == Number(arrSplit[3])) && (dtmBirth.getDate() == Number(arrSplit[4]));
        if (!bGoodDay) {
            return false;
        }
        else {
            //将15位身份证转成18位
            //校验位按照ISO 7064:1983.MOD 11-2的规定生成，X可以认为是数字10。
            var arrInt = new Array(7, 9, 10, 5, 8, 4, 2, 1, 6, 3, 7, 9, 10, 5, 8, 4, 2);
            var arrCh = new Array('1', '0', 'X', '9', '8', '7', '6', '5', '4', '3', '2');
            var nTemp = 0, i;
            num = num.substr(0, 6) + '19' + num.substr(6, num.length - 6);
            for (i = 0; i < 17; i++) {
                nTemp += num.substr(i, 1) * arrInt[i];
            }
            num += arrCh[nTemp % 11];
            return true;
        }
    }
    if (len == 18) {
        re = new RegExp(/^(\d{6})(\d{4})(\d{2})(\d{2})(\d{3})([0-9]|X)$/);
        var arrSplit = num.match(re);

        //检查生日日期是否正确
        var dtmBirth = new Date(arrSplit[2] + "/" + arrSplit[3] + "/" + arrSplit[4]);
        var bGoodDay;
        bGoodDay = (dtmBirth.getFullYear() == Number(arrSplit[2])) && ((dtmBirth.getMonth() + 1) == Number(arrSplit[3])) && (dtmBirth.getDate() == Number(arrSplit[4]));
        if (!bGoodDay) {
            return false;
        }
        else {
            //检验18位身份证的校验码是否正确。
            //校验位按照ISO 7064:1983.MOD 11-2的规定生成，X可以认为是数字10。
            var valnum;
            var arrInt = new Array(7, 9, 10, 5, 8, 4, 2, 1, 6, 3, 7, 9, 10, 5, 8, 4, 2);
            var arrCh = new Array('1', '0', 'X', '9', '8', '7', '6', '5', '4', '3', '2');
            var nTemp = 0, i;
            for (i = 0; i < 17; i++) {
                nTemp += num.substr(i, 1) * arrInt[i];
            }
            valnum = arrCh[nTemp % 11];
            if (valnum != num.substr(17, 1)) {
                return false;
            }
            return true;
        }
    }
    return false;
};




//说明：javascript的乘法结果会有误差，在两个浮点数相乘的时候会比较明显。这个函数返回较为精确的乘法结果。  
function accMul(arg1, arg2) {
    var m = 0, s1 = arg1.toString(), s2 = arg2.toString();
    try { m += s1.split(".")[1].length } catch (e) { }
    try { m += s2.split(".")[1].length } catch (e) { }
    return Number(s1.replace(".", "")) * Number(s2.replace(".", "")) / Math.pow(10, m);
}

//除法函数，用来得到精确的除法结果  
//说明：javascript的除法结果会有误差，在两个浮点数相除的时候会比较明显。这个函数返回较为精确的除法结果。  
//调用：accDiv(arg1,arg2)  
//返回值：arg1除以arg2的精确结果  
function accDiv(arg1, arg2) {
    var t1 = 0, t2 = 0, r1, r2;
    try { t1 = arg1.toString().split(".")[1].length } catch (e) { }
    try { t2 = arg2.toString().split(".")[1].length } catch (e) { }
    with (Math) {
        r1 = Number(arg1.toString().replace(".", ""));
        r2 = Number(arg2.toString().replace(".", ""));
        return (r1 / r2) * pow(10, t2 - t1);
    }
}

//返回两位小数
function changeTwoDecimal_f(x) {
    var f_x = parseFloat(x);
    if (isNaN(f_x)) {
        alert('您试图将字符串转换为数字，请检查!');
        return false;
    }
    var f_x = Math.round(x * 100) / 100;
    var s_x = f_x.toString();
    var pos_decimal = s_x.indexOf('.');
    if (pos_decimal < 0) {
        pos_decimal = s_x.length;
        s_x += '.';
    }
    while (s_x.length <= pos_decimal + 2) {
        s_x += '0';
    }
    return s_x;
}

//计算两个时间所差的天数
function diffDate(str1, str2) {
    str1 = str1.replace(/-/g, "/");
    str2 = str2.replace(/-/g, "/");
    var d1;
    var d2;
    var diffday = 0;
    if (str1 == "") {
        d1 = new Date();
    } else {
        d1 = new Date(str1);
    }
    if (str2 == "") {
        d2 = new Date();
    } else {
        d2 = new Date(str2);
    }
    diffday = Date.parse(d1) - Date.parse(d2);
    diffday = diffday.toFixed(2) / 86400000;
    return diffday;
}

function getDateNow() {
    var now = new Date(); //获取系统日期，即Sat Jul 29 08:24:48 UTC+0800 2006 
    var yy = now.getFullYear(); //截取年，即2006 
    var mo = now.getMonth() + 1; //截取月，即07 
    var dd = now.getDate(); //截取日，即29 
   
    var returnValue = yy + '-' + mo + '-' + dd ;
    return returnValue;
}

function getTimeNow() {
    var now = new Date(); //获取系统日期，即Sat Jul 29 08:24:48 UTC+0800 2006 
    var yy = now.getFullYear(); //截取年，即2006 
    var mo = now.getMonth() + 1; //截取月，即07 
    var dd = now.getDate(); //截取日，即29 
    //取时间 
    var hh = now.getHours(); //截取小时，即8 
    var mm = now.getMinutes(); //截取分钟，即34 
    var ss = now.getSeconds(); //获取秒 
    var returnValue = yy + '-' + mo + '-' + dd + ' ' + hh + ':' + mm + ':' + ss;
    return returnValue;
}

function getDate(date) {
    var now = new Date(date); //获取系统日期，即Sat Jul 29 08:24:48 UTC+0800 2006 
    var yy = now.getFullYear(); //截取年，即2006 
    var mo = now.getMonth() + 1; //截取月，即07 
    var dd = now.getDate(); //截取日，即29 

    var returnValue = yy + '-' + mo + '-' + dd;
    return returnValue;
}

function getTime(time) {
    var now = new Date(time); //获取系统日期，即Sat Jul 29 08:24:48 UTC+0800 2006 
    var yy = now.getFullYear(); //截取年，即2006 
    var mo = now.getMonth() + 1; //截取月，即07 
    var dd = now.getDate(); //截取日，即29 
    //取时间 
    var hh = now.getHours(); //截取小时，即8 
    var mm = now.getMinutes(); //截取分钟，即34 
    var ss = now.getSeconds(); //获取秒 
    var returnValue = yy + '-' + mo + '-' + dd + ' ' + hh + ':' + mm + ':' + ss;
    return returnValue;
}

// 获取日期中的年月
function getDateNow_YM() {
    var now = new Date(); //获取系统日期，即Sat Jul 29 08:24:48 UTC+0800 2006 
    var yy = now.getFullYear(); //截取年，即2006 
    var mo = now.getMonth() + 1; //截取月，即07 
   // var dd = now.getDate(); //截取日，即29 
   
    var returnValue = '';
    if (mo < 10) {
        returnValue = yy.toString() +'0'+ mo.toString();
    } else {
        returnValue = yy.toString() +  mo.toString();
    } 
    return returnValue;
}


// 获取日期中的年月日
function getDateNow_YMD()
    {var now = new Date(); //获取系统日期，即Sat Jul 29 08:24:48 UTC+0800 2006 
        var yy = now.getFullYear(); //截取年，即2006 
        var mo = now.getMonth() + 1; //截取月，即07 
        var dd = now.getDate(); //截取日，即29 
   
        var returnValue = '';
        if (mo < 10) {
            returnValue = yy.toString() + '0' + mo.toString();
        } else {
            returnValue = yy.toString() + mo.toString();
        }
        if (dd < 10) {
            returnValue +=  '0' + dd.toString();
        } else {
            returnValue +=  dd.toString();
        }
        return returnValue;
}

//生成新的GUID
function newGuid() {
    var guid = "";
    for (var i = 1; i <= 32; i++) {
        var n = Math.floor(Math.random() * 16.0).toString(16);
        guid += n;
        if ((i == 8) || (i == 12) || (i == 16) || (i == 20))
            guid += "-";
    }
    return guid;
}

//将数字转换为人民币大写形式
function changeNumMoneyToChinese(money) {
    var cnNums = new Array("零", "壹", "贰", "叁", "肆", "伍", "陆", "柒", "捌", "玖"); //汉字的数字
    var cnIntRadice = new Array("", "拾", "佰", "仟"); //基本单位
    var cnIntUnits = new Array("", "万", "亿", "兆"); //对应整数部分扩展单位
    var cnDecUnits = new Array("角", "分", "毫", "厘"); //对应小数部分单位
    var cnInteger = "整"; //整数金额时后面跟的字符
    var cnIntLast = "元"; //整型完以后的单位
    var maxNum = 999999999999999.9999; //最大处理的数字
    var IntegerNum; //金额整数部分
    var DecimalNum; //金额小数部分
    var ChineseStr = ""; //输出的中文金额字符串
    var parts; //分离金额后用的数组，预定义
    if (money == "") {
        return "";
    }
    money = parseFloat(money);
    if (money >= maxNum) {
        alert('超出最大处理数字');
        return "";
    }
    if (money == 0) {
        ChineseStr = cnNums[0] + cnIntLast + cnInteger;
        return ChineseStr;
    }
    money = money.toString(); //转换为字符串
    if (money.indexOf(".") == -1) {
        IntegerNum = money;
        DecimalNum = '';
    } else {
        parts = money.split(".");
        IntegerNum = parts[0];
        DecimalNum = parts[1].substr(0, 4);
    }
    if (parseInt(IntegerNum, 10) > 0) { //获取整型部分转换
        var zeroCount = 0;
        var IntLen = IntegerNum.length;
        for (var i = 0; i < IntLen; i++) {
            var n = IntegerNum.substr(i, 1);
            var p = IntLen - i - 1;
            var q = p / 4;
            var m = p % 4;
            if (n == "0") {
                zeroCount++;
            } else {
                if (zeroCount > 0) {
                    ChineseStr += cnNums[0];
                }
                zeroCount = 0; //归零
                ChineseStr += cnNums[parseInt(n)] + cnIntRadice[m];
            }
            if (m == 0 && zeroCount < 4) {
                ChineseStr += cnIntUnits[q];
            }
        }
        ChineseStr += cnIntLast;
        //整型部分处理完毕
    }
    if (DecimalNum != '') { //小数部分
        var decLen = DecimalNum.length;
        for (var i = 0; i < decLen; i++) {
            var n = DecimalNum.substr(i, 1);
            if (n != '0') {
                ChineseStr += cnNums[Number(n)] + cnDecUnits[i];
            }
        }
    }
    if (ChineseStr == '') {
        ChineseStr += cnNums[0] + cnIntLast + cnInteger;
    } else if (DecimalNum == '') {
        ChineseStr += cnInteger;
    }
    return ChineseStr;
}

//当前的字符是否为空
function isNull(obj) {
    if (obj == undefined || obj == null || obj == '') {
        return true;
    } else {
        return false;
    }
}

function showBodyCenter(obj) {
    $(obj).css('position','fixed');
    var width = $(obj).width();
    var height = $(obj).height();
    var top = (document.documentElement.clientHeight - height) / 2;
    var left = (document.documentElement.clientWidth - width) / 2;
    if (top <= 0) { top = 10; }
    if (left <= 0) { left = 10; }
    $(obj).css('top', top).css('left', left);
    $(obj).fadeIn("fast");
}


//提示框
function showMsg(msg) {
    if ($('.msgOverlay').length > 0) {
        $('.msgOverlay').remove();
        $('.mesg-window').remove();
    }
    var width = window.screen.width;
    var height = window.screen.height;
    var tagOverlay = '<div class="msgOverlay"></div>';
    $('body').append(tagOverlay);
    $('.msgOverlay').height(height);
    $('.msgOverlay').width(width);
    var tag = '<div class="mesg-window" id="mesgShow">';
    tag += '    <div class="mesg-header">';
    tag += '        <span style="color: #fff">操作提示</span><a class="btn-close right">×</a>';
    tag += '     </div>';
    tag += '    <div class="mesg-content">';
    tag += '        <div class="mesg-cont"></div>';
    tag += '        <a href="javascript:;" class="altokbtn">确认</a>';
    tag += '    </div>';
    tag += ' </div>';
    //$('.msgOverlay').append(tag);
    $('body').append(tag);
    $("#mesgShow .mesg-cont").html("").html(msg);

    /*关闭提示框*/
    $(".mesg-window .btn-close").click(function () {
        $(".msgOverlay").remove();
        $(".mesg-window").remove();
    });
    $(".mesg-window .altokbtn").click(function () {
        $(".msgOverlay").remove();
        $(".mesg-window").remove();
    });

    $(".msgOverlay").show();
}

//确认框
function showConfirm(msg, callback) {
    if ($('.msgOverlay').length > 0) {
        $('.msgOverlay').remove();
        $('.mesg-window').remove();
    }
    var width = window.screen.width;
    var height = window.screen.height;
    var tagOverlay = '<div class="msgOverlay"></div>';
    $('body').append(tagOverlay);
    $('.msgOverlay').height(height);
    $('.msgOverlay').width(width);
    var tag = '<div class="mesg-window" id="mesgShow-confirm">';
    tag += '    <div class="mesg-header"><span style="color: #fff">操作提示</span></div>';
    tag += '    <div class="mesg-content">';
    tag += '        <div class="mesg-cont"></div>';
    tag += '        <a href="javascript:;" class="altokbtn-confirm">确认</a><a href="javascript:;" class="cancelbtn-confirm">取消</a>';
    tag += '    </div>';
    tag += ' </div>';
    //$('.msgOverlay').append(tag);
    $('body').append(tag);
    $(".mesg-window .mesg-cont").html("").html(msg);
    $(".msgOverlay").show();
    /*关闭提示框*/
    $('.mesg-window .altokbtn-confirm').unbind('click').click(function () {
        if (callback) {
            callback('yes');
            $(".msgOverlay").remove();
            $(".mesg-window").remove();
        }
    })
    $('.mesg-window .cancelbtn-confirm').unbind('click').click(function () {
        if (callback) {
            callback('no');
            $(".msgOverlay").remove();
            $(".mesg-window").remove();
        }
    })
}
//for example
//       function funConfirm() {
//               var msg = 'Hello,我是新的确认对话框!';
//               showConfirm(msg, function (obj) {
//                   if (obj == 'yes') {
//                       alert('你点击了确定！');
//} else {
//                       alert('你点击了取消！');
//}

//})
//}

//var i = 0;
//$(function () {
//    setInterval('getNewMsg()', 10000);//每10s获取一次更新
//});

//function getNewMsg() {

//    var msglist = localStorage.getItem('msglist');
//    if (msglist != null && msglist != '') {
//        var divPushMsg = $('.divPushMsg');
//        if (divPushMsg.length == 0) {
//            ShowPushMsg(msglist);
//        }
//        //将未读消息设为已读 
//        //PostNoticeIsRead();
//    }
//}

//function ShowPushMsg(data) {

//    // var noticedata = JSON.parse(data).data.notice;
//    var noticedata = JSON.parse(data)
//    if (noticedata.length == 0) {
//        alert("没有查找到新的未读消息！");
//    }

//    var ID = noticedata.ID;
//    var Title = noticedata.Title;
//    var CreateTime = new Date(noticedata.CreateTime);
//    var strDate = CreateTime.getFullYear() + '-' + (parseInt(CreateTime.getMonth()) + 1) + '-' + CreateTime.getDate();
//    var Content = noticedata.Content;

//    var bodywidth = document.body.scrollWidth;
//    var bodyheight = document.body.scrollHeight;
//    var divwidth = 400;
//    var divheight = 400;
//    var targetleft = parseInt(bodywidth) - parseInt(divwidth) - 30;
//    var targettop = parseInt(bodyheight) - parseInt(divheight) - 30;
//    var divmsg = '';
//    divmsg += '<div class="divPushMsg">';
//    divmsg += ' <div class="divPushMsg_close"> <input type="button" id="noticeclose" value="×"></body></div>';
//    divmsg += ' <div class="divPushMsg_title"><span onclick="ShowDetail(\'' + ID + '\')">' + Title + '</span></div>';
//    divmsg += '   <div class="divPushMsg_content"></div>';
//    divmsg += ' <div class="divPushMsg_time"></div>';
//    divmsg += '  <div class="divPushMsg_confirm"><p id="pushmsgid">'+ID+'</p><a href="#" onclick="PostNoticeIsRead(\''+ID+'\')">已阅</a></div> ';
//    divmsg += ' </div>';
//    $('body').append(divmsg);
//    //$('.divPushMsg .divPushMsg_title').html(Title);
//    $('.divPushMsg .divPushMsg_content').html(Content);
//    $('.divPushMsg .divPushMsg_time').html(strDate);
//   // $('.divPushMsg .divPushMsg_confirm #pushmsgid').html(ID);//消息ID
//    $('.divPushMsg').css('left', bodywidth).css('top', bodyheight).css('width', divwidth).css('height', divheight);
//    $('.divPushMsg').fadeIn();
//    $('.divPushMsg').animate({
//        left: targetleft,
//        top: targettop,
//        opacity: '1'
//    }, "slow");

//    $('.divPushMsg .divPushMsg_close #noticeclose').click(function () {
//        //$('.divPushMsg').fadeOut();
//        $('.divPushMsg').remove();
//    })
//}


//function ShowDetail(id) {
//    var msglist = localStorage.getItem('msglist');
//    var width = 960;
//    var height = 600;
//    var top = (window.screen.height - height) / 2;
//    var left = (window.screen.width - width) / 2;
//    localStorage.setItem('pushmsg', msglist);
//    localStorage.setItem('msglist', '');//将未读消息列表置空
//    var url = '/fs/pushmessage.html';
//    $('.divPushMsg').remove();
//    window.open(url, 'newwindow', 'height=' + height + ', width=' + width + ', top=' + top + ', left=' + left + ', toolbar=no, menubar=no, scrollbars=no, resizable=no, location=no, status=no');

//}

//function PostNoticeIsRead(id) {
//    $('.divPushMsg').remove();
//    var webSiteCode = localStorage.getItem('webSiteCode');
//    var apiurl = localStorage.getItem('apiurl');
//        var noticeInfo = {
//            webSiteCode: webSiteCode,//站点代码
//            noticeID: id,//公告系统编号
//            isRead: "true"//已读标识（true 或者 false）
//        }
//        var apiUri = apiurl + "/api/Notice/PostNoticeIsRead";//请使用绝对访问路径，参数：webSiteCode-站点代码，maxNum-获取最多条目数
// //查看其它网点对此消息的阅读状态

//        $.ajax({
//            url: '/Ashx/wbinfo.ashx?type=UpdateWBPushMsgState&msgid=' + id,
//            type: 'post',
//            contentType: 'application/json',
//            async: true,
//            data: '',
//            dataType: 'json',
//            beforeSend: function (r) { },
//            success: function (r) {
//                localStorage.setItem('msglist', '');//将未读消息置空
//                if (r == "1") {//所有的网点已全部读取此信息
//                    $.ajax({
//                        url: apiUri,
//                        type: 'post',
//                        contentType: 'application/json',
//                        async: true,
//                        data: JSON.stringify(noticeInfo),
//                        dataType: 'json',
//                        beforeSend: function (r) { },
//                        success: function (r) {
//                            console.log('success!');
//                        },
//                        error: function (r) { console.log('error!'); }
//                    });
//                }
//            },
//            error: function (r) { }
//        });

        
//    }


