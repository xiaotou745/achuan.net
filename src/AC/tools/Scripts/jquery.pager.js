
(function ($) {
    $.fn.pager = function (options) {
        var opts = $.extend({}, $.fn.pager.defaults, options);
        return this.each(function () {
            $(this).empty().append(renderpager(parseInt(options.pagenumber), parseInt(options.pagecount),
            parseInt(options.totalRecords), options.buttonClickCallback));
            $("#btnpagerTo").keydown(function (event) {
                if (event.keyCode == 13) {
                    var tempval = $(this).val();
                    if ((parseInt(tempval) <= parseInt(options.pagecount)) && parseInt(tempval) != 0) {
                        options.buttonClickCallback(tempval);
                    }
                }
            });
        });
    };
    function renderpager(pagenumber, pagecount, totalRecords, buttonClickCallback) {
        var $pager = $('<div class="Samplepages"></div>');
        if (pagenumber == 1) {
            $pager.append("<label>首页</label>").append("<label>上一页</label>");
        }
        else {
            $pager.append(renderButton('首页', pagenumber, pagecount, buttonClickCallback))
        .append(renderButton('上一页', pagenumber, pagecount, buttonClickCallback));
        }
        var startPoint = 1;
        var endPoint = 9;
        if (pagenumber > 4) {
            startPoint = pagenumber - 4;
            endPoint = pagenumber + 4;
        }
        if (endPoint > pagecount) {
            startPoint = pagecount - 8;
            endPoint = pagecount;
        }
        if (startPoint < 1) {
            startPoint = 1;
        }
        for (var page = startPoint; page <= endPoint; page++) {
            var currentButton = $('<label class="page-number">' + (page) + '</label>');
            page == pagenumber ? (currentButton = $('<label class="page-number">' + (page) + '</label>')) : currentButton = $('<a href="#" class="page-number">' + (page) + '</a>');
            page == pagenumber ? currentButton.addClass('pgCurrent') : currentButton.click(function () { buttonClickCallback(this.firstChild.data); });
            currentButton.appendTo($pager);
        }
        if (pagecount == pagenumber || (pagecount == 0 && pagenumber == 1)) {
            $pager.append("<label>下一页</label>");
        }
        else {
            $pager.append(renderButton('下一页', pagenumber, pagecount, buttonClickCallback));
        }
        $pager.append("<label>共" + pagecount + "页</label>")
        .append("<label>到第</label><input type='text' id='btnpagerTo' style='width:25px;' /><label>页</label>");
        if (pagecount == pagenumber || (pagecount == 0 && pagenumber == 1)) {
            $pager.append("<label>末页</label>");
        }
        else {
            $pager.append(renderButton('末页', pagenumber, pagecount, buttonClickCallback));
        }
        $pager.append("<label>" + totalRecords + "条记录</label>");
        return $pager;
    }
    function renderButton(buttonLabel, pagenumber, pagecount, buttonClickCallback) {
        var $Button = $('<a href="#">' + buttonLabel + '</a>');
        var $curBtn = $("<label>" + buttonLabel + "</label>");
        var destPage = 1;
        switch (buttonLabel) {
            case "首页":
                destPage = 1;
                break;
            case "上一页":
                destPage = pagenumber - 1;
                break;
            case "下一页":
                destPage = pagenumber + 1;
                break;
            case "末页":
                destPage = pagecount;
                break;
        }
        if (buttonLabel == "首页" || buttonLabel == "上一页") {
            pagenumber <= 1 ? $Button.addClass('pgEmpty') : $Button.click(function () { buttonClickCallback(destPage); });
        }
        else {
            pagenumber >= pagecount ? $Button.addClass('pgEmpty') : $Button.click(function () { buttonClickCallback(destPage); });
        }
        return $Button;
    }
    $.fn.pager.defaults = {
        pagenumber: 1,
        pagecount: 1,
        totalRecords: 0
    };
})(jQuery);








