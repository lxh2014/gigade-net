var Time_status;
var Time_num;
editFunction = function (row, store) {

    
    var editFrm = Ext.create('Ext.form.Panel', {
        id: 'editFrm',
        frame: true,
        plain: true,
        layout: 'anchor',
        autoScroll: true,
        labelWidth: 45,
        url: '/PromotionsBonus/InsertPromotionsBonusSerial2',
        defaults: { anchor: "95%", msgTarget: "side", labelWidth: 80 },
        items: [
            {
                xtype: 'numberfield',
                editable: true,
                fieldLabel: '序號數量',
                allowNegative: false,
                minValue: 1,
                value:1,
                hideTrigger:true,
                allowDecimals:false,
                labelWidth: 80,
                emptyValue: 1,
                id: 'xhsl',
                name: 'xhsl'
            },
            {
                xtype: 'numberfield', 
               
                minValue: 5,
                maxValue: 32,
                fieldLabel: '序號長度',
                labelWidth: 80,
                id: 'xhcds',
                name: 'xhcds',
                value: 8,
                emptyValue: 8,
                editable: true,
                allowNegative: false,
                hideTrigger:true,
                allowDecimals:false
            },
             {
                 fieldLabel: 'row_id',
                 xtype: 'textfield',
                 editable: false,
                 id: 'row_id',
                 name: 'row_id',
                 hidden: true,
                 value: row
             }
        ],
        buttons: [{
            text: "保存",
            formBind: true,
            disabled: true,
            handler: function () {
                var form = this.up('form').getForm();
                var nums = Ext.getCmp("xhsl").getValue();
                var lengt= Ext.getCmp("xhcds").getValue();
               
                if (nums == null || lengt == null || nums == "" || lengt =="")
                {
                    Ext.Msg.alert("提示", "請輸入序號數量和長度!");
                    return;
                }
                Time_status = true;
                Time_num = 0;
                thisNumster();
               
               // var Son = setInterval('thisSucces(' + nums + ')', 1000);
                if (form.isValid()) {
                    form.submit({
                        timeout: 900000,
                        params: {
                            xhsl:nums,
                            xhcd: lengt,
                            ids: Ext.getCmp("row_id").getValue()
                        },
                        success: function (form, action) {
                            Time_status = false;
                            var result = Ext.decode(action.response.responseText);
                            if (result.success) {
                                Ext.Msg.alert("提示信息", "保存成功！");
                                FaresStore.load();
                                editWin.close();
                            }
                            else {
                                Ext.Msg.alert("提示信息", "保存失敗！");
                                editWin.close();
                            }
                        },
                        failure: function () {
                            Time_status = false;
                            Ext.Msg.alert("提示信息", "等待超時！");
                        }
                    });
                }
            }
        }
        ]
    });

 
    var editWin = Ext.create('Ext.window.Window', {
        title: "新增序號",
        id: 'editWin',
        iconCls: "icon-user-add",
        width: 350,
        height: 220,
        layout: 'fit',
        items: [editFrm],
        constrain: true, //束縛窗口在框架內
        closeAction: 'destroy',
        modal: true,
        resizable: false,
        labelWidth: 60,
        bodyStyle: 'padding:5px 5px 5px 5px',
        closable: false,
        tools: [
         {
             type: 'close',
             qtip: "關閉窗口",
             handler: function (event, toolEl, panel) {
                 Ext.MessageBox.confirm("提示信息", "是否關閉窗口", function (btn) {
                     if (btn == "yes") {
                         Ext.getCmp('editWin').destroy();
                     }
                     else {
                         return false;
                     }
                 });
             }
         }]
    });
    editWin.show();
}

function thisSucces(nums) {
        //Ext.Ajax.request({
        //    url: "/PromotionsBonus/GetInsertNum",
        //    method: 'post',
        //    type: 'text',
        //    params: { },
        //    success: function (form, action) {
        //        var result = Ext.decode(form.responseText);
        //        if (result.success) {
                   
        //            Ext.Msg.alert("提示", "已新增" + result.msg + "/" + nums + "條!");
        //        }
        //        else {
        //            Ext.Msg.alert("提示", "異常");
        //        }
        //    }
        //});
        
   //     setTimeout('thisSucces('+nums+')',1000);
    //}
}
function thisNumster() {
    if (Time_status)
    {
        Time_num = Time_num + 1;
        Ext.Msg.alert("提示", "已執行" + Time_num + "秒!");
        setTimeout("thisNumster()", 1000);
    }
}