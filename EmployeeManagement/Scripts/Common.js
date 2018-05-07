var rootUrl = "";
//Start Checkbox select
var CheckboxCheck = function (tableName) {
    $('#' + tableName + ' input[id="chkRow"]').click(function () {
        if ($('#' + tableName + ' input[id="chkRow"]').length == $('input[id="chkRow"]:checked').length)
            $('#' + tableName + ' #chkAll').prop('checked', true);
        else
            $('#' + tableName + ' #chkAll').prop('checked', false);

        $(this).closest('tbody tr').toggleClass("active", this.checked);
    });

    $('#' + tableName + ' #chkAll').click(function () {
        $('#' + tableName + ' input:checkbox[id="chkRow"]').prop('checked', $('#' + tableName + ' #chkAll').is(':checked'));
        $('#' + tableName + ' tbody tr').toggleClass("active", this.checked);
    });

    $('#' + tableName + ' tbody tr td:not(:first-child)').click(function (event) {
        //if (event.target.type !== 'checkbox') {
        //$(':checkbox', this).trigger('click');
        //}
        $(this).closest('tr').find('td:first-child :checkbox').trigger('click')
    });



}
//End Checkbox select

//Start Add/Edit/Delete Item
var AddItem = function (btnName, tableName, url, param, dvModel) {
    $("#" + btnName).click(function () {
        GetModelData(btnName, tableName, url, param, dvModel, 0);
    });
}
var EditSelectedItem = function (btnName, tableName, url, param, dvModel) {
    $("#" + btnName).click(function () {
        var id = [];
        var chk = $("#" + tableName + " tr td input[id='chkRow']:checkbox:checked");
        var count = 0;
        if (chk != null && chk != '') {
            count = chk.length;
        }
        if (count == 0) {
            alert("Select Item.");
        }
        else if (count > 1) {
            alert("Select Only Item.");
        }
        else {
            var newParam = param.replace("[replaceid]", chk.val());//[replaceId] with selected table id            
            GetModelData(btnName, tableName, url, newParam, dvModel, chk.val());
        }
    });
}
var EditItemOnDoubleClick = function (tableName, url, param, dvModel) {
    $('#' + tableName + ' tbody tr').dblclick(function (e) {
        var id = $(this).find('input:checkbox[id="chkRow"]').val();
        var newParam = param.replace("[replaceid]", id);//[replaceId] with selected table id            
        GetModelData("", tableName, url, newParam, dvModel, id);
    });
}
function GetModelData(btnName, tableName, url, param, dvModel, id) {
    $("#hdnRowupdateIndex").val(id);//Save id on hiddenfield for highlight row after update         
    //alert(btnName + " : " + tableName + " : " + url + " : " + param + " : " + dvModel);
    $.ajax({
        type: "POST",
        url: url,
        data: param,
        contentType: "application/json; charset=utf-8",
        success: function (data) {
            $("#lblmainmsg").html("");
            $('#' + dvModel).html(data);
            $('#' + dvModel).modal('show');
            //Close model popup
            CloseModelPopup();

            formValidation();//Validation
            SetDatepicker();//Set Datepicker
        },
        error: function (ex) {
            console.log(ex);
        }
    });
}
var DeleteSelectedItem = function (btnName, tableName, url, param, dvList) {
    $("#" + btnName).click(function () {
        var ids = ""; var i = 0;
        var chk = $("#" + tableName + " tr td input[id='chkRow']:checkbox:checked");
        var count = 0;
        if (chk != null && chk != '') {
            count = chk.length;
        }
        if (count == 0) {
            alert("Select item for delete.");
        }
        else {
            $('#' + tableName + ' tr').each(function () {
                if ($(this).find("input[id*='chkRow']").length > 0) {
                    if ($(this).find("input[id*='chkRow']")[0].checked == true) {
                        ids = ids + $(this).find("input[id*='chkRow']").val() + ",";
                    }
                }
            });

            var newParam = param.replace("[replaceid]", ids);//[replaceId] with selected table id            
            if (confirm("Are you sure want delete selected items ?")) {
                $.ajax({
                    type: "POST",
                    url: url,
                    data: newParam,
                    contentType: "application/json; charset=utf-8",
                    success: function (data) {
                        $("#" + dvList).html(data);
                    },
                    error: function (ex) {
                    }
                });
            }
        }
    });
}
//End Add/Edit/Delete Item

//Start Grid
var needToSearch = false;
$(document).on('click', function (event) {
    //if (lName != "") {
    $("#condition").hide();
    //alert($(event.target).closest('#' + tableList).length + " :: " + needToSearch);
    if (!$(event.target).closest('#' + tableList).length && needToSearch) {

        var ControllerName = ''; var ActionName = ''; var tableList = ''; var tableName = '';
        var arr = $(event.target).closest('#' + tableList).find("#hdnParamNames").val().split('|');//Ex:  ControllerName|ActionName|tableList|tableName
        if (arr != null) { ControllerName = arr[0]; ActionName = arr[1]; tableList = arr[2]; tableName = arr[3]; }

        Search(ControllerName, ActionName, tableList, tableName);
        needToSearch = false;
    }
    //}
});

var checkState = function (obj) {
    var cblock = $("#condition");
    if ($(obj).data("pastvalue") != $(obj).val()) {
        $(obj).data("pastvalue", $(obj).val());
        needToSearch = true;
    }
}

var bindGridEvent = function (tableName) {
    //alert(tableName + " :: "+ $("#" + tableName).attr('class'));
    InitCheckboxAndDoubleClick();//Initialize function

    $("#" + tableName + " thead th a[id*='filter']").click(function (e) {
        var cblock = $("#condition");
        if (cblock.is(":hidden") || cblock.data("element") != this.id) {
            cblock.data("element", this.id);
            var top = $(this).offset().top - 35;//40
            var left = $(this).offset().left + $(this).width() / 2 - (cblock.width() / 2);
            cblock.css({ top: top, left: left });

            var input = $(this).closest("th").find("input[type='text']");
            var input2 = $(this).closest("th").find("input[type='checkbox']");
            $("ul li.selected", cblock).removeClass("selected");
            $("ul li[data-value='" + input.data("condition") + "']", cblock).addClass("selected");
            $("ul li[data-value='" + input2.data("condition") + "']", cblock).addClass("selected");

            if ((input && input.data("column")) && (input.data("column").indexOf("CreatedOn") > -1 || input.data("column").match("Date"))) {
                $("ul li[data-type='date']", cblock).show();
                $("ul li[data-type='string']", cblock).hide();
                $("ul li[data-type='bool']", cblock).hide();
            }
            else if ((input2 && input2.data("column")) && (input2.data("column").match("IsActive") || input2.data("column").match("IsUserVisible") || input2.data("column").match("IsDefault"))) {
                $("ul li[data-type='bool']", cblock).show();
                $("ul li[data-type='string']", cblock).hide();
                $("ul li[data-type='date']", cblock).hide();
            }
            else {
                $("ul li[data-type='string']", cblock).show();
                $("ul li[data-type='date']", cblock).hide();
                $("ul li[data-type='bool']", cblock).hide();
            }

            cblock.show();
            e.stopPropagation();
            e.preventDefault();
        } else {
            cblock.hide();
        }
    });


    $("#" + tableName + " thead th input[type='text']").keydown(function (e) {
        if (e.keyCode == 13 && $(this).data("pastvalue") != $(this).val().trim()) {
            var ControllerName = ''; var ActionName = ''; var tableList = ''; var tableName = '';
            var arr = $(this).closest('table').find("#hdnParamNames").val().split('|');//Ex:  ControllerName|ActionName|tableList|tableName
            if (arr != null) { ControllerName = arr[0]; ActionName = arr[1]; tableList = arr[2]; tableName = arr[3]; }
            Search(ControllerName, ActionName, tableList, tableName);//Filter Search
        }
    });

    $("#" + tableName + " thead th div input[data-column*='Date']").datetimepicker({ timepicker: false, format: 'Y-m-d' });
    $("#" + tableName + " thead th div input[data-column*='CreatedOn']").datetimepicker({ timepicker: false, format: 'Y-m-d' });
    $("#" + tableName + " thead th div input[data-column*='DueBy']").datetimepicker({ timepicker: false, format: 'Y-m-d' });
}

var FilterCondition = function (conditionName, ControllerName, ActionName, tableList, tableName) {
    $("#" + conditionName + " li").click(function (e) {
        var div = $("#" + conditionName);
        var input = $("#" + div.data("element")).siblings(':first'); //$("#" + div.data("element")).closest("th").find("input[type='text']");
        if (input.attr('type') == "checkbox") {
            var chkVal = input.prop("checked");
            if ($(this).data("value") == "No") {
                input.data("condition", $(this).data("value"));
                $(this).data("value", "");
                input.prop("checked", false);
                Search(ControllerName, ActionName, tableList, tableName);//Filter Search
            } else if (input.data("condition") != $(this).data("value")) {
                input.data("condition", $(this).data("value"));
                Search(ControllerName, ActionName, tableList, tableName);//Filter Search
            } else {
                input.data("condition", $(this).data("value"));
            }
        }
        else {
            //alert(input.data("condition") + " :: " + $(this).data("value") + " :: " + input.val().trim());
            //No :: Contain :: 11
            if (input && $(this).data("value") == "No" && input.val().trim()) {
                input.val('');
                input.data("condition", "No");
                Search(ControllerName, ActionName, tableList, tableName);
            } else if (input.data("condition") != $(this).data("value") && input.val().trim()) {
                input.data("condition", $(this).data("value"));
                Search(ControllerName, ActionName, tableList, tableName);
            } else if (condition != $(this).data("value") && ($(this).data("value") == "is null" || $(this).data("value") == "is not null")) {
                input.data("condition", $(this).data("value"));
                Search(ControllerName, ActionName, tableList, tableName);
            } else if (input && $(this).data("value") == "No" && input.val().trim() == "" && input.data("condition")) {
                input.val('');
                input.data("condition", "No")
                Search(ControllerName, ActionName, tableList, tableName);
            }
            else {
                input.data("condition", $(this).data("value"));
            }
        }
        $(".selected").removeClass("selected");
        $(this).addClass("selected");
        div.hide();
        e.stopPropagation();
        e.preventDefault();
    });
}

var FilterChange = function ($this) { Search(); }

var setSortorder = ""; var setFieldName = "";
var SortSetting = function (sOrder, fName) { setSortorder = sOrder; setFieldName = fName; }

function Search(ControllerName, ActionName, tableList, tableName) {
    needToSearch = false;
    var para = new Array();
    $("#" + tableName + " thead th").each(function () {
        var input = $(this).find("input[type='text']");
        if (input && input.data("column") && input.data("condition") && input.val().trim()) {
            para.push({ ColumnName: input.data("column"), Condition: input.data("condition"), Value: input.val() });
        } else if (input.data("condition") == "is null" || input.data("condition") == "is not null") {
            para.push({ ColumnName: input.data("column"), Condition: input.data("condition"), Value: input.val() });
        } else if (input && input.data("column") && input.val().trim()) {
            para.push({ ColumnName: input.data("column"), Condition: "No", Value: input.val() });
        }
        var input1 = $(this).find("select");
        //alert(input1 + " :: "+input1.val());
        if (input1 && input1.data("column") && input1.data("condition") && input1.val().trim() != '' && input1.val().length != 0) {
            para.push({ ColumnName: input1.data("column"), Condition: input1.data("condition"), Value: input1.val() });
        }
        var inputchk = $(this).find("input[type=checkbox]:not('#chkAll')");
        if (inputchk && inputchk.data("column") && inputchk.data("condition") && inputchk.data("condition") != 'No') {
            para.push({ ColumnName: inputchk.data("column"), Condition: inputchk.data("condition"), Value: inputchk.prop("checked") });
        }
    });

    //perserve sort order = fieldName: setFieldName, sortOrder: setSortorder
    var pagesize = $("#" + tableName).find(".pagerDropdown").val();
    var searchType = "";//$("#txtMainSearch").val();
    var searchText = '';
    var hdnViewtype = $("#ViewType").val();
    $.ajax({
        type: "POST",
        contentType: 'application/json',
        url: rootUrl + ControllerName + "/" + ActionName,
        data: JSON.stringify({ SearchText: searchText, SearchType: searchType, Filters: para, pageSize: pagesize, fieldName: setFieldName, sortOrder: setSortorder, ViewType: hdnViewtype }),
        success: function (data) {
            $("#" + tableList).html(data);
            bindGridEvent(tableName);
        },
        beforeSend: function (jqXHR, settings) {
            //if (lName == "IncidentTable" || lName == "ProblemTable" || lName == "SolutionTable" || lName == "ChangeList" || lName == "AssetTable") {
            //beforePage(jqXHR, settings);
            //}
        },
    });
}

function beforePage(jqXHR, settings, oldPagesize, tblName) {
    //alert($(this).closest('div').next('table').attr('id'));
    var ControllerName = ''; var ActionName = ''; var tableList = ''; var tableName = '';
    var arr = '';
    //alert($(this).attr('id'));
    if (oldPagesize != null && oldPagesize != '' && oldPagesize != undefined) {//call when pager dropdown to change pagesize

        var arr = $('#' + tblName).find("#hdnParamNames").val().split('|');//Ex:  ControllerName|ActionName|tableList|tableName     

    } else {
        if ($(this).closest('table').find("#hdnParamNames").val() != undefined) {//call when sorting/paging
            var arr = $(this).closest('table').find("#hdnParamNames").val().split('|');//Ex:  ControllerName|ActionName|tableList|tableName
        }
        else {//call when submit CRUD operation form fro highlight row after add/update
            var arr = $("#" + $(this).data('tablename')).find("#hdnParamNames").val().split('|');//Ex:  ControllerName|ActionName|tableList|tableName
        }
    }
    if (arr != null || arr != '') { ControllerName = arr[0]; ActionName = arr[1]; tableList = arr[2]; tableName = arr[3]; }//Ex:  ControllerName|ActionName|tableList|tableName

    //alert(ControllerName + " :: " + ActionName + " :: " + tableList + " :: " + tableName + " :: ");

    var para = '';
    var i = 0;    
    if (settings.url.indexOf("?") >= 0)
        settings.url = settings.url + '&';
    else
        settings.url = settings.url + '?';
    var pSize = "10";
    if ($("#" + tableList).find(".pagerDropdown").length > 0) {
        pSize = $("#" + tableList).find(".pagerDropdown").val();
    }
    var page = "1";
    if (oldPagesize == undefined || pSize == oldPagesize) {//page =1 because if already on last page and change pagesize , so there is not exists last page, so prevent error , goes to 1st page        
        if ($("#" + tableName + " ul.pagination").find(".active").length > 0) {
            page = $("#" + tableName + " ul.pagination").find(".active").text();
        }
    }
    var searchText = '';
    if ($("#filterDropDown") != null || $("#filterDropDown") != undefined)
    { searchText = $("#filterDropDown").val(); } //Common name for main filter search(dropdown or textbox) in incident/Solution


    para += 'SearchText=' + searchText;
    if (settings.url.indexOf("page=") >= 0) { } else { para += '&page=' + page; }
    para += '&pageSize=' + pSize;
    para += '&fieldName=' + setFieldName + '&sortOrder=' + setSortorder;
    $("#" + tableList + " table th").each(function () {
        var input = $(this).find("input[type='text']");
        if (input.data("column") && input.data("condition") && input.data("condition") != "No") {
            para += '&Filters[' + i + '].ColumnName=' + input.data("column");
            para += '&Filters[' + i + '].Condition=' + input.data("condition");
            para += '&Filters[' + i + '].Value=' + encodeURIComponent(input.val());
            i++;
        }
        var input1 = $(this).find("select");
        if (input1.data("column") && input1.data("condition") && input1.data("value") != "No" && input1.data("value") != "" && input1.data("value").length != 0 && input1.val() != "") {
            para += '&Filters[' + i + '].ColumnName=' + input1.data("column");
            para += '&Filters[' + i + '].Condition=' + input1.data("condition");
            para += '&Filters[' + i + '].Value=' + encodeURIComponent(input1.val());
            i++;
        }
        var input2 = $(this).find("input[type='checkbox']");
        if (input2.data("column") && input2.data("condition")) {
            var val = input2.prop("checked");
            para += '&Filters[' + i + '].ColumnName=' + input2.data("column");
            para += '&Filters[' + i + '].Condition=' + input2.data("condition");
            para += '&Filters[' + i + '].Value=' + encodeURIComponent(val);
            i++;
        }
    });
    
    settings.url = settings.url + para;    
}

function pagerDropdownChange(obj, url, oldPagesize) {
    var tblName = $(obj).closest('table').attr('id');
    $.ajax({
        url: url,
        method: "POST",
        success: function (data) {
            if ($(obj).data("ajax-update")) {
                $($(obj).data("ajax-update")).html(data);

                //save pagesize setting
                //var modulename = "";
                //if (lName == "IncidentTable") { modulename = "Incident"; }
                //else if (lName == "ProblemTable") { modulename = "Problem"; }
                //else if (lName == "ChangeList") { modulename = "Change"; }
                //else if (lName == "SolutionTable") { modulename = "Solution"; }
                //else if (lName == "AssetTable") { modulename = "Asset"; }
                //if (modulename != "") {
                //    var isStaticPageSize = $("#CategoryList").find("#hdnIsStaticPageSize").val();
                //    if (isStaticPageSize.toLowerCase() == "false") {
                //        var pageSize = $("#CategoryList").find(".pagerDropdown").val();
                //        $.ajax({
                //            type: "POST",
                //            dataType: "json",
                //            url: rootUrl + "Home/InsertPagesizeSetting",
                //            data: { 'isStaticPageSize': isStaticPageSize, 'pageSize': pageSize, 'ModuleName': modulename },
                //            success: function (data) {
                //            }
                //        });
                //    }
                //}
            }
        },
        beforeSend: function (jqXHR, settings) { if ($(obj).data("ajax-begin")) { window[$(obj).data("ajax-begin")](jqXHR, settings, oldPagesize, tblName); } },
        complete: function () {
            if ($(obj).data("ajax-complete")) {
                var callback_function = new Function($(obj).data("ajax-complete"));
                callback_function();//this function call with parameterize function like test('hello')
                //window[$(obj).data("ajax-complete")]();//this function call without parameterize function like test
            }
        },
        error: function () { if ($(obj).data("ajax-failure")) window[$(obj).data("ajax-failure")](); },
    });
}
//End Grid









































