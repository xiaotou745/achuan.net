//通用方法
//CreatTime:2012-09-21
//EditRecord:[No.][Editor][EditTime][Remark]
// 1.
// 2.
// 3.
//处理平台JS问题
var PlatformUrl = "http://sascs.vancldb.com/ExecuteChildJS";
$(function () {
    $("#ui-datepicker-div").hide();
});


//isAsync参数可选，默认为true异步
function CallAction(url, data, successFunc, isAsync) {
    if (isAsync == undefined || isAsync == null)
        isAsync = true;
    $.ajax({
        //提交数据的类型 POST GET
        type: "POST",
        //提交的网址
        url: url,
        //提交的数据
        data: data,
        cache: false,
        async: isAsync,
        //返回数据的格式
        datatype: "json", //"xml", "html", "script", "json", "jsonp", "text".
        //在请求之前调用的函数
        //beforeSend:function(){$("#msg").html("logining");},
        //成功返回之后调用的函数            
        success: function (data, textStatus, XMLHttpRequest) {

            if (data && data.toString().indexOf("<!DOCTYPE html") > 0) {
                if (top.location != self.location) {
                    top.location = self.location;
                }
                window.location.href = "/Login/Login";
                return;
            }
            successFunc(data);
            var innerHtml = '<div id="errorInfo"></div>'
            $("#errorInfo").replaceWith(innerHtml);
        },

        //调用出错执行的函数
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            $.unblockUI();
            //请求出错处理
            var realErr = $("#errorWrap", XMLHttpRequest.responseText).html();
            if (realErr && realErr.length > 1) {
                alert("请求出错：" + realErr);
            }
            else {
                alert("异步调用出错");
            }


        }
    });
}








Dialog = {
    /**把Div 用Dialog方式 弹出的方法  【需要在页面load中调用 ，即在$(function(){});中调用此方法，然后在要弹出时使用$("#DivID").dialog("open"); 即可弹出】
    *2011-07-28
    *zhangshanchun
    *描述：eleName--Div 的ID
    *width--弹出Dialog的 Width ，默认为500，如果使用默认值，此参数传null
    *height--弹出Dialog 的Height，默认为400，如果使用默认值，此参数传null
    *funs--要显示的Button 及其执行的function 集合 
    *如：funs={"确定":fun1,"取消":fun2},其中fun1,fun2为方法（如果在执行完方法的代码后需要关闭dialog的，
    *则需要在方法代码的最后添加  $(this).dialog("close");  语句）
    *调用例子：Dialog.dialogDiv("abc", null,null, { "确定": function () { alert(1); $(this).dialog("close");}, "全选": function () { alert(2); }, "取消": function () { alert(3); } });
    */
    dialogDiv: function (eleName, width, height, funs) {
        if (width == null) {
            width = 500;
        }
        if (height == null) {
            height = 400;
        }
        $("#" + eleName).dialog(
            {
                width: width,
                height: height,
                autoOpen: false,
                modal: true,
                draggable: false,
                resizable: false,
                buttons: funs
            });
    },
    dialogIframe: function (url, title, width, height, isOuter) {
        var horizontalPadding = 10;
        var verticalPadding = 10;
        if (isOuter != null && typeof isOuter == "boolean" && isOuter == true) {
            width = width || 1100;
            height = height||500;
            window.parent.$('<iframe id="dialogIframeID" src="' + url + '" style="overflow-y:scroll;overflow-y:auto;" frameborder=\"no\" border=\"0\" marginwidth=\"0\" marginheight=\"0\"  />').dialog({
                title: title ? title : '',
                autoOpen: true,
                width: width,
                height: height,
                close: function (event, ui) { $("#dialogIframeID").remove(); },
                modal: true,
                draggable: true,
                resizable: false
            }).width(width - horizontalPadding).height(height - verticalPadding);
        } else {
            if (typeof width == "undefined") {
                width = 500;
            }
            if (typeof height == "undefined") {
                height = 400;
            }
            $('<iframe id="dialogIframeID" src="' + url + '" style="overflow-y:scroll;overflow-y:auto;" frameborder=\"no\" border=\"0\" marginwidth=\"0\" marginheight=\"0\"  />').dialog({
                title: title ? title : '',
                autoOpen: true,
                width: width,
                height: height,
                close: function (event, ui) { $("#dialogIframeID").remove(); },
                modal: true,
                draggable: true,
                resizable: false
            }).width(width - horizontalPadding).height(height - verticalPadding);
        }
    },
    dialogIframeClose: function () {
        $("#dialogIframeID").dialog("destroy");
        $("#dialogIframeID").remove();
    }
};

//#region Cookie操作 liud
//根据名称获取cookie
function GetCookie(c_name) {
    if (document.cookie.length > 0) {
        c_start = document.cookie.indexOf(c_name + "=");
        if (c_start != -1) {
            c_start = c_start + c_name.length + 1;
            c_end = document.cookie.indexOf(";", c_start);
            if (c_end == -1) c_end = document.cookie.length;
            return unescape(document.cookie.substring(c_start, c_end));
        }
    }
    return "";
}
function SetCookie(name, value) {
    var Days = 30;
    var exp = new Date();
    exp.setTime(exp.getTime() + Days * 24 * 60 * 60 * 1000);
    document.cookie = name + "=" + escape(value) + ";expires=" + exp.toGMTString();
}
function DelCookie(name) {
    var exp = new Date();
    exp.setTime(exp.getTime() - 1);
    var cval = getCookie(name);
    if (cval != null) document.cookie = name + "=" + cval + ";expires=" + exp.toGMTString();
}

//#endregion

//#region 日期处理函数 liud
//格式：2012-01-02 12：12
function parseMSJSONString(cellvalue, options, rowObject) {
    if (cellvalue != null && cellvalue != "") {
        var date = new Date(parseInt(cellvalue.replace("/Date(", "").replace(")/", ""), 10));
        var month = padLeft(date.getMonth() + 1, 10);
        var currentDate = padLeft(date.getDate(), 10);
        var H = date.getHours();
        var M = date.getMinutes();
        return date.getFullYear() + "-" + month + "-" + currentDate + " " + H + ":" + M;
    }
    return "-";
}
//格式：2012-01-02
function parseMSJSONStringNoHM(cellvalue, options, rowObject) {
    if (cellvalue != null && cellvalue != "") {
        if (cellvalue.indexOf("/Date") >= 0) {
            var date = new Date(parseInt(cellvalue.replace("/Date(", "").replace(")/", ""), 10));
            var month = padLeft(date.getMonth() + 1, 10);
            var currentDate = padLeft(date.getDate(), 10);
            return date.getFullYear() + "-" + month + "-" + currentDate;
        } else {
            return cellvalue;
        }
    }
    return "-";
}
function padLeft(str, min) {
    if (str >= min)
        return str;
    else
        return "0" + str;
}
//#endregion

//#region 页面操作日志 liud
function WebOperationLog(className) {
    $(className).bind("click", function () {
        $.ajax({
            type: "POST",
            url: "/WebOperationLog/LogInfo",
            data: {
                DomID: $(this).attr("id"),
                DomName: $(this).val() == "" ? $(this).attr("text") : $(this).val(),
                PageTitle: $(document).attr('title'),
                RequestURL: window.location.href
            },
            datatype: "json",
            success: function (data, textStatus, XMLHttpRequest) {
            },
            Error: function () {
            }
        });
    });
}
//#endregion

//#region 封装confirm liud
function CommConfirm(message, callback) {
    if (message == null) {
        message = "确认要执行此操作吗？执行后不可回退！";
    }
    if (!confirm(message)) {
        return;
    }
    callback();
}
//#endregion =======
//#endregion

//#region 页面操作日志 liud
function WebOperationLog(className) {
    $(className).bind("click", function () {
        $.ajax({
            type: "POST",
            url: "/WebOperationLog/LogInfo",
            data: {
                DomID: $(this).attr("id"),
                DomName: $(this).val() == "" ? $(this).attr("text") : $(this).val(),
                PageTitle: $(document).attr('title'),
                RequestURL: window.location.href
            },
            datatype: "json",
            success: function (data, textStatus, XMLHttpRequest) {
            },
            Error: function () {
            }
        });
    });
}


function GetResultTip(tip) {
    return "<span style='color:red'>"+tip+"</span>"
}

//#region 处理脚本跨域执行问题 
//
//
//

function parentFun(tabid, url, title, width, height, func) {
    var external = true;
    try {
        external = parent.location.host != location.host;
    }
    catch (e) {
        external = true;
    }
    if (!external) {
        parent.openExternalTab(tabid, url, title);
        return;
    }
    var body = $("body");

    if ($("#_opentab").length < 1) {
        $("<iframe id=\"_opentab\" name=\"_opentab\" style=\"display:none\"></iframe>").appendTo(body);
    }
    if (url.indexOf(location.host) == -1) {
        url = location.protocol + "//" + location.host + url;
    }

    var form = $("<form style=\"display:none\" id=\"opentabForm\" method=\"POST\" target=\"_opentab\" action=\"" + PlatformUrl + "\" />");
    var i_url = $("<input type=\"text\" name=\"url\" \>").val(url).appendTo(form);
    var i_title = $("<input type=\"text\" name=\"title\" \>").val(title).appendTo(form);
    var i_tabid = $("<input type=\"text\" name=\"tabid\" \>").val(tabid).appendTo(form);
    var i_width = $("<input type=\"text\" name=\"width\" \>").val(width + 20).appendTo(form);
    var i_height = $("<input type=\"text\" name=\"height\" \>").val(height + 20).appendTo(form);
    var i_funName = $("<input type=\"text\" name=\"functionName\" \>").val(func).appendTo(form);

    form.appendTo(body);
    form.submit();
    form.remove();
}

