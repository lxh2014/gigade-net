editFunction = function (Order_id, Order_Status) {
    var editFrm = Ext.create('Ext.form.Panel', {
        id: 'editFrm',
        frame: true,
        plain: true,
        defaultType: 'textfield',
        layout: 'anchor',
        labelWidth: 45,
        url: '/OrderManage/ReturnAllOrder',
        defaults: { anchor: "95%", msgTarget: "side" },
        items: [
              {
                  html: '1.取消訂單後，系統將<font color=red>回加庫存量</font>。</br>2.取消訂單後，<font color=red>若該筆訂單有使用購物金折抵，系統將自動歸還購物金</font>。</br>3.<font color=red>※取消訂單後，該筆訂單不允許再做任何更改</font>。'
                  // bodyStyle: "padding:5px;background:#7DC64C",
                 ,
                  height: 80,
                  border: 0

              },
        
        {
            xtype: 'displayfield',
            name: 'return_Order_id',
            id: 'return_Order_id',
            submitValue: true,
            margin: '5,0,5,0',
            maxLength: 25,
            fieldLabel: '付款單號'
        },
        {
            xtype: 'displayfield',
            name: 'return_order_status',
            id: 'return_order_status',
            margin: '5,0,5,0',
            allowBlank: false,
            submitValue: true,
            fieldLabel: '目前訂單狀態'
        },
       
        {
            xtype: 'textareafield',
            name: 'return_note',
            id: 'return_note',
            margin: '5,0,5,0',
            fieldLabel: '備註',
            allowBlank: false,
            maxLength:100,
            submitValue: true
        },
          {
              html: '※限制100個中文字，<font  color=red>管理員備註用，不開放前台會員查詢</font>'
                 ,
              height: 40,
              border: 0

          }
        ],
        buttons: [
            {
                text: SAVE,
                formBind: true,
                disabled: true,
                handler: function () {
                    var form = this.up('form').getForm();
                    if (form.isValid()) {
                        var myMask = new Ext.LoadMask(Ext.getBody(), { msg: 'Loading...' });
                        myMask.show();
                        form.submit({
                            params: {
                                return_order: Ext.htmlEncode(Ext.getCmp('return_Order_id').getValue()),
                                order_note: Ext.htmlEncode(Ext.getCmp('return_note').getValue()),
                                
                            },
                            success: function (form, action) {
                                var result = Ext.decode(action.response.responseText);
                                if (result.success) {
                                    myMask.hide();
                                    if (result.msg !="100") {
                                        Ext.Msg.alert(INFORMATION, result.msg);
                                        editWin.close();
                                    }
                                    else {
                                        myMask.hide();
                                        Ext.Msg.alert(INFORMATION, SUCCESS);
                                        TranToDetial(Ext.htmlEncode(Ext.getCmp('return_Order_id').getValue()));
                                        //editWin.close();
                                    }
                                } else {
                                    myMask.hide();
                                    Ext.Msg.alert(INFORMATION, FAILURE);
                                }
                            },
                            failure: function () {
                                myMask.hide();
                                Ext.Msg.alert(INFORMATION, FAILURE);
                            }
                        });
                    }
                }
            }]
    });
    var editWin = Ext.create('Ext.window.Window', {
        title: '取消訂單',
        iconCls: 'icon-user-edit',
        id: 'editWin',
        width: 400,
        y: 100,
        layout: 'fit',
        items: [editFrm],
        constrain: true,
        closeAction: 'destroy',
        modal: true,
        resizable: false,
        labelWidth: 60,
        bodyStyle: 'padding:5px 5px 5px 5px',
        closable: false,
        tools: [
         {
             type: 'close',
             qtip: '是否關閉',
             handler: function (event, toolEl, panel) {
                 Ext.MessageBox.confirm(CONFIRM, IS_CLOSEFORM, function (btn) {
                     if (btn == "yes") {
                         Ext.getCmp('editWin').destroy();
                     }
                     else {
                         return false;
                     }
                 });
             }
         }],
        listeners: {
            'show': function () {
                var id = Order_id;             
                Ext.getCmp('return_Order_id').setValue(Order_id);
                Ext.getCmp('return_order_status').setValue(Order_Status);
                Ext.Ajax.request({
                    url: "/OrderManage/ReturnMsg",                  
                    params: {
                        order_id: Order_id
                    },
                    success: function (response) {
                        var result = Ext.decode(response.responseText);
                       
                        if (result.msg != "100")
                        {
                            Ext.Msg.alert("提示", result.msg);
                        }
                    }
                });
            }
        }
    });
    editWin.show();
}