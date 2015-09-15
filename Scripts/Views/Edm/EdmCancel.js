//取消電子報
Ext.onReady(function () {
    var exportTab = Ext.create('Ext.form.Panel', {
        layout: 'anchor',
        width: 600,
        url: '/Edm/CancelEdm',
        margin: '0 10 0 0',
        defaults: {
            labelWidth: 80,
            width: 400,
            margin: '20 10 0 20'
        },
        border: false,
        plain: true,
        id: 'exportTab',
        items: [
            {
                xtype: 'panel',
                bodyStyle: "padding:5px;background:#EEEEEE",//background:#87CEEB
                border: false,
                html: "注意事項：<br/>此功能會取消指定mail所有電子報！"
            },
             {
                 xtype: 'textfield',
                 fieldLabel: '請輸入EMAIL',
                 id: 'mail',
                 name: 'mail',
                 vtype: 'email',
                 submitValue: true,
                 allowBlank: false
             },
             {
                 xtype: 'displayfield',
                 id: 'vid',
                 name: 'vid',
                 hidden: true
             }

        ],
        buttonAlign: 'right',
        buttons: [{
            text: '送出',
            formBind: true,
            disabled: true,
            handler: function () {
                var form = this.up('form').getForm();
                if (form.isValid()) {
                    this.disable();
                    Ext.MessageBox.confirm(CONFIRM, "確認取消該mail的所有電子報？", function (btn) {
                        if (btn == "yes") {
                            var myMask = new Ext.LoadMask(Ext.getBody(), { msg: 'Loading...' });
                            myMask.show();
                            form.submit({
                                params: {
                                    mail: Ext.htmlEncode(Ext.getCmp('mail').getValue())
                                },
                                success: function (form, action) {
                                    var result = Ext.decode(action.response.responseText);                                    
                                    myMask.hide();
                                    if (result.success) {
                                        Ext.Msg.alert(INFORMATION, SUCCESS);
                                        exportTab.getForm().reset(); //如果是添加的話
                                    }
                                    else {
                                        Ext.getCmp('mail').reset();
                                        if (result.msg == '0') {
                                            Ext.Msg.alert(INFORMATION, "該Email不存在！");
                                        }
                                        else if (result.msg == '1') {
                                            Ext.Msg.alert(INFORMATION, "已取消電子報，但用戶不是會員，無法加入黑名單！");
                                        }
                                        else if (result.msg == '2') {
                                            Ext.MessageBox.confirm(CONFIRM, "已取消電子報并加入黑名單，是否將狀態改變為鎖定？", function (btn) {
                                                if (btn == "yes") {
                                                    Ext.getCmp('vid').setValue(Ext.decode(action.response.responseText).vid),
                                                    ok();                                                  
                                                }
                                            }
                                            )
                                        }
                                        else if (result.msg == '3') {
                                            Ext.Msg.alert(INFORMATION, "該用戶已取消電子報且存在于黑名單,狀態為：鎖定");
                                        }                                      
                                        else {
                                            Ext.Msg.alert(INFORMATION, FAILURE);
                                        }
                                    }

                                },
                                failure: function (form, action) {
                                    Ext.getCmp('mail').reset();
                                    if (Ext.decode(action.response.responseText).msg == 0) {
                                        Ext.Msg.alert(INFORMATION, "該Email不存在！");
                                        Ext.getCmp('mail').reset();
                                    } else if (Ext.decode(action.response.responseText).msg == '1') {
                                        Ext.Msg.alert(INFORMATION, "已取消電子報，但用戶不是會員，無法加入黑名單！");
                                    }
                                    else if (Ext.decode(action.response.responseText).msg == '2') {
                                        Ext.MessageBox.confirm(CONFIRM, "已取消電子報且该邮箱在黑名单中的状态为：解除。是否將狀態改變為鎖定？", function (btn) {
                                            if (btn == "yes") {
                                                Ext.getCmp('vid').setValue( Ext.decode(action.response.responseText).vid),
                                                ok();
                                            }
                                        }
                                        )
                                    }
                                    else if (Ext.decode(action.response.responseText).msg == '3') {
                                        Ext.Msg.alert(INFORMATION, "該用戶已取消電子報且存在于黑名單,狀態為：鎖定");
                                    }
                                    else if (Ext.decode(action.response.responseText).msg == '4') {
                                        Ext.Msg.alert(INFORMATION, "該郵箱不存在于群組中，無法取消電子報！");
                                    }
                                    else {
                                        Ext.Msg.alert(INFORMATION, FAILURE);
                                    }
                                    myMask.hide();
                                
                                }
                            })
                        }
                    })
                }
            }
        }
        ]
    });
    Ext.create('Ext.Viewport', {
        layout: 'hbox',
        items: [exportTab],
        renderTo: Ext.getBody(),
        autoScroll: true,
        listeners: {
            resize: function () {
                this.doLayout();
            }
        }
    });
    ToolAuthority();

});
function ok() {
    Ext.Ajax.request({
        url: '/Edm/UpdateState',
        method: 'post',
        params: {
            id: Ext.getCmp('vid').getValue(),
            active: 1
        },
        success: function (msg) {
            var result = eval("(" + msg.responseText + ")");
            var m = result.msg;
            if (m == 1) {
                Ext.Msg.alert(INFORMATION, SUCCESS);
            }
            else {
                Ext.Msg.alert(INFORMATION, FAILURE);
            }
        },
        error: function (msg) {
            Ext.Msg.alert(INFORMATION, FAILURE);           
        }
    });
}
