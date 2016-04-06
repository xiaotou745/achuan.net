$(document).ready(function () {
    $(window).prettyPrint && prettyPrint();

    var dbName = window.dbname;
    if (dbName == "") {
        $("#txtDbName").focus();
    } else {
        $("#txtTableName").focus();
    }
    $("#dbServer").on("change", dbServerChanged);
    $("#txtDbName").on("keydown", dbNameKeydown);
    $("#txtTableName").on("keydown", tableNameKeydown);
    //$("#btnCodeSettings").on("click", showCodeGenerateSetting);
    //$("#btnJavaCodeSettings").on("click", showCodeGenerateSetting);
    $("#ddlCallStyle,#ddlCodeLayer,#ddlDaoStyle").change(function () {
        var title = $(this).find("option:selected").attr("title");
        $(this).next("p").text(title);
    });
    $("#ddlCodeLayer").change();
    $("#ddlCallStyle").change();
    $("#ddlDaoStyle").change();

    $("#btnCodeSettings,#btnJavaCodeSettings").on("click", function (sender, e) {
        $("#lblTableName").val($("#txtTableName").val());
        $("#codeType").val($(this).data("code"));
        $("#generateCodeSettings").modal({
            backdrop: true,
            keyboard: true,
            show: true
        });
    });

    $("#btnGenerateCode").on("click", function () {
        generateCode();
    });
    
    $("#btnGenerateCodeAndDownload").on("click", function () {
        generateCodeAndDownload();
    });

    $("#tabResult > ul li").addClass("hide");
});
function generateCodeAndDownload() {
    var params = {
        dbServer: $("#tdDbServer").text(),
        dbName: $("#tdDbName").text(),
        tableName: $("#tdTableName").text(),
        callStyle: $("#ddlCallStyle").val(),
        codeLayer: $("#ddlCodeLayer").val(),
        daoStyle: $("#ddlDaoStyle").val(),
        modelName: $("#tdTableName").text(),
    };

    $.blockUI();
    $.post("/CodeGenerate/GenerateCodeAndDownload",
        params,
        function (data, textStatus) {
            if (data.IsSuccess) {
                alert(data.FilePath);
            }
            $.unblockUI();
        }
    );
}
function dbServerChanged() {
    $("#txtDbName").val("");
    $("#txtTableName").val("");
    $("#dbSelector").submit();
}

function dbNameKeydown(evt) {
    evt = (evt) ? evt : ((window.event) ? window.event : "");
    var keyCode = evt.keyCode ? evt.keyCode : (evt.which ? evt.which : evt.charCode);
    if (keyCode != 13) {
        return;
    }
    var dbName = $("#txtDbName").val();
    if (dbName == "") {
        $("#txtDbName").focus();
        return;
    }
    $("#txtTableName").val("");
    $("#dbSelector").submit();
}

function tableNameKeydown(evt) {
    evt = (evt) ? evt : ((window.event) ? window.event : "");
    var keyCode = evt.keyCode ? evt.keyCode : (evt.which ? evt.which : evt.charCode);
    if (keyCode != 13) {
        return;
    }
    setTableInfo($("#dbServer").val(), $("#txtDbName").val(), $("#txtTableName").val(), "");
    getTableInfo($("#dbServer").val(), $("#txtDbName").val(), $("#txtTableName").val(), "");
}

function setTableInfo(dbServer, dbName, tableName, tableDesc) {
    $("#tdTableName").text(tableName);
    $("#tdDbServer").text(dbServer);
    $("#tdDbName").text(dbName);
    $("#tdTableDesc").text(tableDesc);
}

function getTableInfo(dbServer, dbName, tableName, tableDesc) {
    $.blockUI();
    $.post("/TableView/GetHtmlTableInfo",
        { dbServer: dbServer, dbName: dbName, dbTable: tableName },
        function (data) {
            setTableInfo(dbServer, dbName, tableName, tableDesc);
            $("#tdTableInfo").show();

            $("#tabResult > ul li").removeClass("active");
            $("#tabResult > .tab-content > div").removeClass("active");
            $("#liColumnInfo").addClass("active").show();
            $("#tabColumnInfo").addClass("active");

            $("#tabColumnInfo").html(data);
            $.unblockUI();
        }
    );
}

function generateCode() {
    var params = {
        dbServer: $("#tdDbServer").text(),
        dbName: $("#tdDbName").text(),
        tableName: $("#tdTableName").text(),
        callStyle: $("#ddlCallStyle").val(),
        codeLayer: $("#ddlCodeLayer").val(),
        daoStyle: $("#ddlDaoStyle").val(),
        modelName: $("#tdTableName").text(),
        codeType: $("#codeType").val()
    };
    $.blockUI();
    $.post("/CodeGenerate/GenerateCode",
        params,
        function (data, textStatus) {
            if (data.IsSuccess) {
                if (data.ServiceCode) {
                    $("#tabCodes a[href=#tabService]").show();
                    $("#codeOfService").text(data.ServiceCode);
                } else {
                    $("#tabCodes a[href=#tabService]").hide();
                    $("#codeOfService").text("");
                }

                if (data.ServiceImplCode) {
                    $("#tabCodes a[href=#tabServiceImpl]").show();
                    $("#codeOfServiceImpl").text(data.ServiceImplCode);
                } else {
                    $("#tabCodes a[href=#tabServiceImpl]").hide();
                    $("#codeOfServiceImpl").text("");
                }

                if (data.ServiceDTOCode) {
                    $("#tabCodes a[href=#tabServiceDTO]").show();
                    $("#codeOfServiceDTO").text(data.ServiceDTOCode);
                } else {
                    $("#tabCodes a[href=#tabServiceDTO]").hide();
                    $("#codeOfServiceDTO").text("");
                }

                if (data.DaoCode) {
                    $("#tabCodes a[href=#tabDao]").show();
                    $("#codeOfDao").text(data.DaoCode);
                } else {
                    $("#tabCodes a[href=#tabDao]").hide();
                    $("#codeOfDao").text("");
                }

                if (data.DomainCode) {
                    $("#tabCodes a[href=#tabDomain]").show();
                    $("#codeOfDomain").text(data.DomainCode);
                } else {
                    $("#tabCodes a[href=#tabDomain]").hide();
                    $("#codeOfDomain").text("");
                }
                if (data.MyBatis) {
                    $("#tabCodes a[href=#tabMyBatis]").show();
                    $("#codeOfBatis").text(data.MyBatis);
                } else {
                    $("#tabCodes a[href=#tabMyBatis]").hide();
                    $("#codeOfBatis").text("");
                }
                $("#generateCodeSettings").modal("hide");
                $("#tdTableInfo").show();
                $("#tabResult > ul li").removeClass("active");
                $("#tabResult > .tab-content > div").removeClass("active");
                $("#liCodes").addClass("active").show();
                $("#tabCodes").addClass("active");
            }
            $.unblockUI();
        }
    );
}
