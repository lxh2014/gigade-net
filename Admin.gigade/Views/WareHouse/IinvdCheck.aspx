<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<!DOCTYPE html>

<html>
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1, user-scalable=yes " />
    <script src="../../Scripts/jquery-2.1.4.min.js"></script>
    <link href="../../Scripts/Bootstrap/css/bootstrap.min.css" rel="stylesheet" />
    <script src="../../Scripts/Bootstrap/bootstrap.min.js"></script>
    <script src="../../Scripts/Bootstrap/bootstrap-table.js"></script>
    <script src="../../Scripts/Bootstrap/bootstrap-datetimepicker.min.js"></script>
    <link href="../../Scripts/Bootstrap/css/bootstrap-datetimepicker.min.css" rel="stylesheet" />
    <script src="../../Scripts/Bootstrap/locales/bootstrap-datetimepicker.zh-TW.js"></script>
    <title>IinvdCheck</title>
</head>
<body>
    <div class="container">
        <h1>請開始盤點商品數量</h1>
        <br />
        <h3>料位編號: <span class="label label-info" id="loc_id"><%=ViewBag.loc_id%></span></h3>
        <h3>商品名稱: <span class="label label-info">(<%=ViewBag.item_id%>) <%=ViewBag.product_name%> <%=ViewBag.spec%> 條碼:<%=ViewBag.upc_id %></span></h3>
        <span id="item_id" hidden="hidden"><%=ViewBag.item_id%></span>
        <span id="pwy_dte_ctl" hidden="hidden"><%=ViewBag.pwy_dte_ctl%></span>
        <span id="iplas" hidden="hidden"><%=ViewBag.iplas%></span>
        <br />
        <div class="bs-example bs-example-padded-bottom">
            <button type="button" class="btn btn-primary" data-toggle="modal" data-target="#myModal" onclick="reset()">新增庫存</button>
        </div>
        <table class="table table-bordered table-striped table-hover" id="Table1">
            <%if(ViewBag.count!=0){ %>
            <thead class="lead">
                <tr>
                    <%if (ViewBag.pwy_dte_ctl == "Y" && ViewBag.item_id != "此料位暫無商品" &&ViewBag.lcat_id=="")
                      { %>
                    <th style="text-align: center; vertical-align: middle;" data-field="made_date" tabindex="0">
                        <div class="th-inner ">
                            製造日期                   
                        </div>
                        <div class="fht-cell"></div>
                    </th>
                    <th style="text-align: center; vertical-align: middle;" data-field="cde_dt" tabindex="0">
                        <div class="th-inner ">
                            有效日期                  
                        </div>
                        <div class="fht-cell"></div>
                    </th>
                    <%} %>
                 <%--   <th style="text-align: center; vertical-align: middle;" data-field="prod_qty" tabindex="0">
                        <div class="th-inner ">
                            現有庫存                   
                        </div>
                        <div class="fht-cell"></div>
                    </th>--%>
                    <th style="text-align: center; vertical-align: middle;" data-field="3" tabindex="0">
                        <div class="th-inner ">
                            實際庫存                 
                        </div>
                        <div class="fht-cell"></div>
                    </th>
                    <th style="text-align: center; vertical-align: middle;" data-field="4" tabindex="0">
                        <div class="th-inner ">
                            操作                 
                        </div>
                        <div class="fht-cell"></div>
                    </th>
                </tr>
            </thead>
            <%} %>
            <tbody>
                <%for (int i = 0; i < ViewBag.count; i++)
                  {
                      if (i % 2 == 0)
                      { %>
                <tr data-index="0">
                    <td style="text-align: center; vertical-align: middle;" hidden="hidden"><%=ViewBag.data[i].row_id%></td>
                    <%if (ViewBag.pwy_dte_ctl == "Y" && ViewBag.item_id != "此料位暫無商品" && ViewBag.lcat_id == "")
                      { %>
                    <td style="text-align: center; vertical-align: middle;"><%=BLL.gigade.Common.CommonFunction.DateTimeToShortString(ViewBag.data[i].made_date)%></td>
                    <td style="text-align: center; vertical-align: middle;"><%=BLL.gigade.Common.CommonFunction.DateTimeToShortString(ViewBag.data[i].cde_dt)%></td>
                    <%}%>
                    <td style="text-align: center; vertical-align: middle;" id="prod_qtys" hidden="hidden"><%=ViewBag.data[i].prod_qty%></td>
                    <td style="text-align: center; vertical-align: middle;">
                        <input min="0" class="form-control" id="input<%=ViewBag.data[i].row_id%>" onkeydown="return check(event);" type="number">
                    </td>
                    <td style="text-align: center; vertical-align: middle;">
                        <%--<button id="add2" class="btn btn-primary btn-ss" onclick="Edit('input<%=ViewBag.data[i].row_id %>');">編輯</button>--%>
                        <button id="add3" class="btn btn-primary btn-ss" onclick="Save('input<%=ViewBag.data[i].row_id%>')">保存</button>
                    </td>
                </tr>
                <%}
                      else
                      { %>
                <tr data-index="1">
                    <td style="text-align: center; vertical-align: middle;" hidden="hidden" id="<%=ViewBag.data[i].row_id%>"></td>
                    <%if (ViewBag.pwy_dte_ctl == "Y" && ViewBag.item_id != "此料位暫無商品" && ViewBag.lcat_id == "")
                      { %>
                    <td style="text-align: center; vertical-align: middle;"><%=BLL.gigade.Common.CommonFunction.DateTimeToShortString(ViewBag.data[i].made_date)%></td>
                    <td style="text-align: center; vertical-align: middle;"><%=BLL.gigade.Common.CommonFunction.DateTimeToShortString(ViewBag.data[i].cde_dt)%></td>
                    <%} %>
                    <td style="text-align: center; vertical-align: middle;"hidden="hidden"><%=ViewBag.data[i].prod_qty%></td>
                    <td style="text-align: center; vertical-align: middle;">
                        <input type="number" min="0" class="form-control" id="input<%=ViewBag.data[i].row_id%>" onkeydown="return check(event);">
                    </td>
                    <td style="text-align: center; vertical-align: middle;">
                        <button id="add4" class="btn btn-primary btn-ss" onclick="Save('input<%=ViewBag.data[i].row_id%>');">保存</button>
                    </td>
                </tr>
                <%}
                  } %>
            </tbody>
        </table>
        <div id="myModal" class="modal fade" tabindex="-1" role="dialog" aria-labelledby="myModalLabel">
            <div class="modal-dialog" role="document">
                <div class="modal-content">
                    <div class="modal-header">
                        <button type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                        <h4 class="modal-title" id="myModalLabel">新增庫存</h4>
                    </div>
                    <div class="modal-body">
                        <%if (ViewBag.pwy_dte_ctl == "Y")
                          { %>
                        <span style="font-size: large;">製造日期：</span><input class="form-control" style="width:157px;float:right;margin-right:320px;" size="21" type="text" id="datetimepicker1" readonly="readonly" onchange="GetDate('datetimepicker1')">
                        <br />
                        <br />
                        <span style="font-size: large;">有效日期：</span><input class="form-control" style="width:157px;float:right;margin-right:320px;" size="21" type="text" id="datetimepicker2" readonly="readonly" onchange="GetDate('datetimepicker2')">
                        <%--<div class="input-group date form_date col-md-5" data-link-field="dtp_input2" style="float:right;margin-right:220px;" ><input class="form-control" size="5" type="text" value="" readonly="readonly" id="datetimepicker2"><span class="input-group-addon"><span class="glyphicon glyphicon-calendar"></span></span></div>--%>
                        <br />
                        <br />
                        <%} %>
                        <%if (ViewBag.pwy_dte_ctl == "N"&&ViewBag.item_id == "此料位暫無商品")
                          { %>
                        <span style="font-size: large;">細項編號：</span><input class="span2" size="16" type="number" id="itemid" min="0" onkeydown="return check(event);" onblur="CheckItem('itemid')">
                        <br /><br />
                        <span style="font-size: large;">商品名稱：</span><span style="font-size: large;" id="productname"></span>
                        <br /><br />
                        <%} %>
                        <span style="font-size: large;">現有庫存：</span><input class="span2" size="16" type="number" id="prod_qty" min="0" onkeydown="return check(event);">
                        <br /><br />
                        <div id="myAlert" class="alert alert-warning" hidden="hidden">
                            <a href="#" class="close" data-dismiss="alert">&times;</a>
                            <strong id="message"></strong>
                        </div>
                    </div>
                    <div class="modal-footer">
                        <button type="button" class="btn btn-primary" onclick="reset()">重置</button>
                        <button type="button" class="btn btn-primary" onclick="SaveIinvd()">保存</button>
                    </div>
                </div>
                <!-- /.modal-content -->
            </div>
            <!-- /.modal-dialog -->
        </div>
        <div id="alert" class="modal fade MarketTally-modal-alert" tabindex="-1" role="dialog" aria-labelledby="myModalLabel">
            <div class="modal-dialog modal-sm" role="document">
                <div class="modal-content">
                    <div class="modal-header">
                        <button type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                        <h4 class="modal-title" id="H1">溫馨提示</h4>
                    </div>
                    <div class="modal-body">
                        <div id="divalert" class="alert alert-warning">
                            <strong id="alertmessage"></strong>
                        </div>
                    </div>
                </div>
                <!-- /.modal-content -->
            </div>
            <!-- /.modal-dialog -->
        </div>

        <div id="alertdatediv" class="modal fade MarketTally-modal-alert" tabindex="-1" role="dialog" aria-labelledby="myModalLabel">
            <div class="modal-dialog modal-sm" role="document">
                <div class="modal-content">
                    <div class="modal-header">
                        <button type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                        <h4 class="modal-title" id="H2">溫馨提示</h4>
                    </div>
                    <div class="modal-body">
                        <div id="div2" class="alert alert-warning">
                            <strong id="alertdate"></strong>
                        </div>
                    </div>
                </div>
                <!-- /.modal-content -->
            </div>
            <!-- /.modal-dialog -->
        </div>
        <div id="alertsuccess" class="modal fade MarketTally-modal-alert" tabindex="-1" role="dialog" aria-labelledby="myModalLabel">
            <div class="modal-dialog modal-sm" role="document">
                <div class="modal-content">
                    <div class="modal-header">
                        <button type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                        <h4 class="modal-title" id="H3">溫馨提示</h4>
                    </div>
                    <div class="modal-body">
                        <div id="div3" class="alert alert-warning">
                            <strong id="Strong1">保存成功！</strong>
                        </div>
                    </div>
                </div>
                <!-- /.modal-content -->
            </div>
            <!-- /.modal-dialog -->
        </div>
        <!-- /.modal -->
         <div class="bs-example bs-example-padded-bottom">
            <button type="button" class="btn btn-primary" onclick="NextIloc()">盤點下一個料位</button>
        </div>
    </div>
</body>
</html>
<script type="text/javascript">
    function NextIloc() {
        location.href = "/WareHouse/IlocCheck";
    }
    function CheckItem(id)
    {
        var item_id = $('#' + id).val();
        if(item_id.trim()!='')
        {
            $.ajax({
                url: "/WareHouse/Getprodbyid",
                type: 'POST',
                dataType: "text",
                data: { id: item_id },
                success: function (data) {
                    var result = eval("(" + data + ")");
                    if (result.success) {
                        msg = result.msg;
                        $('#productname').text(msg);
                    }
                    else {
                        $('#productname').text("沒有該商品信息！");
                    }
                }
            });
        } 
    }
    function GetDate(id)
    {
        var date = $('#' + id).val();
        var dateType = 'cde';
        var item_id = $('#item_id').text();
        if (id == 'datetimepicker1')
        {
            dateType = 'made';
        }
        $.ajax({
            url: "/WareHouse/GetItemDate",
            type: "POST",
            dataType: "text",
            data: { date: date, dateType: dateType, item_id: item_id},
            success: function (data) {
                var result = eval("(" + data + ")");
                if (result.success) {
                    if (dateType == 'made') {
                        $('#datetimepicker2').val(result.date);
                    }
                    else {
                        $('#datetimepicker1').val(result.date);
                    }
                }
            }
        });
    }
    $('#datetimepicker1').datetimepicker({
        language: 'zh-TW',
        format: "yyyy - mm - dd",
        weekStart: 1,
        todayBtn: 1,
        autoclose: 1,
        todayHighlight: 1,
        startView: 2,
        minView: 2,
        forceParse: 0,
        showMeridian: 1
    });
    $('#datetimepicker2').datetimepicker({
        language: 'zh-TW',
        format: "yyyy - mm - dd",
        weekStart: 1,
        todayBtn: 1,
        autoclose: 1,
        todayHighlight: 1,
        startView: 2,
        minView: 2,
        forceParse: 0,
        showMeridian: 1
    });
    var date = new Date();
    var year = date.getFullYear();
    var month = date.getMonth() + 1;
    var day = date.getDate();
    $('#datetimepicker1').datetimepicker('setEndDate', year + '-' + month + '-' + day);
    $('#datetimepicker2').datetimepicker('setStartDate', year + '-' + month + '-' + day);
    function check(e) {
        var keynum
        var keychar
        var numcheck

        if (window.event) // IE
        {
            keynum = e.keyCode
        }
        else if (e.which) // Netscape/Firefox/Opera
        {
            keynum = e.which
        }

        keychar = String.fromCharCode(keynum)
        //numcheck = /\D/
        //console.log(keychar+":"+keynum)
        if ((keynum == 46) || (keynum == 8) || (keynum == 37) || (keynum == 39)) {
            return true;
        }

        if (((keynum >= 48 && keynum <= 57) || (keynum >= 96 && keynum <= 105))) {
            return true;
        }
        return false
    }
    function Save(id) {
        var changeStore = $('#' + id).val();
        var pwy_dte_ctl = $('#pwy_dte_ctl').text();
        var prod_qtys = $('#prod_qtys').text();
        var loc_id = $('#loc_id').text();
        var item_id = $('#item_id').text();
        if (changeStore.trim() == "") return false;
        $.ajax({
            url: "/WareHouse/IinvdSave",
            type: "POST",
            dataType: "text",
            data: { changeStore: changeStore, rowid: id, pwy_dte_ctl: pwy_dte_ctl, prod_qtys: prod_qtys, loc_id: loc_id, item_id: item_id },
            success: function (data) {
                var result = eval("(" + data + ")");
                if (result.success) {
                    $('#' + id).val(''); 
                    $('#alertsuccess').modal('toggle');
                    location.href = "/WareHouse/IinvdCheck?pwy_dte_ctl=" + pwy_dte_ctl + "&loc_id="+loc_id;
                }
                else {
                    $("#alertmessage").text(result.message);
                    $('#alert').modal('toggle');
                }
            }
        });
    }
    function reset() {
        $('#datetimepicker1').val('');
        $('#datetimepicker2').val('');
        $('#prod_qty').val(''); 
        $('#itemid').val('');
        $('#productname').text('');
    }
    function SaveIinvd() {
        var pwy_dte_ctl = $('#pwy_dte_ctl').text();
        var datetimepicker1 = $('#datetimepicker1').val();
        var datetimepicker2 = $('#datetimepicker2').val();
        var prod_qty = $('#prod_qty').val();
        var st_qty = $('#st_qty').val();
        var loc_id = $('#loc_id').text();
        var item_id = $('#item_id').text();
        var itemid = $('#itemid').val();
        var iplas = $('#iplas').text();
        if (pwy_dte_ctl == "Y") {
            if (datetimepicker1.trim() == "" || prod_qty.trim() == "" || datetimepicker2.trim() == "")
                return false;
        }
        else {
            if (prod_qty.trim() == "")
                return false;
            if (item_id.trim() == "此料位暫無商品"&&itemid.trim()=="")
            {
                return false;
            }
        } 
        if (item_id.trim() == "此料位暫無商品")
        {
            item_id = itemid;
        }
        if(iplas=="false")
        {
            var productname = $('#productname').text();
            if (productname == "" || productname == "沒有該商品信息！") return false;
        }
        var date = new Date();
        var year = date.getFullYear();
        var month = date.getMonth() + 1;
        var day = date.getDate();
        var datetime = year + ' - ' + month + ' - ' + day;

        if (datetimepicker1>datetime)
        {
            $("#alertdate").text('製造日期不能大於當前日期');
            $('#alertdatediv').modal('toggle');
            return false;
        }
        if (datetimepicker2 <datetime)
        {
            $("#alertdate").text('有效日期不能小於當前日期');
            $('#alertdatediv').modal('toggle');
            return false;
        }
        $.ajax({
            url: "/WareHouse/SaveIinvd",
            type: "POST",
            dataType: "text",
            data: { datetimepicker1: datetimepicker1, prod_qty: prod_qty, st_qty: st_qty, loc_id: loc_id, item_id: item_id, pwy_dte_ctl: pwy_dte_ctl, iplas: iplas },
            success: function (data) {
                var result = eval("(" + data + ")");
                if (result.success) {
                    $('#datetimepicker1').val('');
                    $('#prod_qty').val('');
                    $('#alertsuccess').modal('toggle');
                    location.reload();
                }
                else {
                    $("#message").text(result.message);
                    $("#myAlert").show();
                    $('#prod_qty').val('');
                }
            }
        });
    }
</script>
