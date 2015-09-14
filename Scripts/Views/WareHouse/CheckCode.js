editFunction = function (msg) {
    var codeFrm = Ext.create('Ext.form.Panel', {
        id: 'codeFrm',
        frame: true,
        plain: true,
        constrain: true,
        defaultType: 'textfield',
        autoScroll: true,
        layout: 'anchor',
        defaults: { anchor: "95%", msgTarget: "side" },
        items: [
            {
                xtype: 'displayfield',
                id: 'info',
                name: 'info',
               // value: '商品日期已經超過有效日期!',
                submitValue: true
            },  
            {
                xtype: 'fieldcontainer',
                combineErrors: true,
                layout: 'hbox',
                items: [

                    {
                        xtype: 'textfield',
                        fieldLabel: '驗證碼',
                        id: 'check_code',
                        name: 'check_code',
                        margin: '5 7 0 5',
                        allowBlank: false,
                        labelWidth: 45,
                        width:150,
                        submitValue: true
                    },
                   {
                       xtype: 'displayfield',
                       name: 'showcode',
                       id: 'showcode',
                       margin: '5 3 0 1',
                       submitValue: true,
                       value:returnCode()
                   },
                ]
            }
        ],
        //buttonAlign: 'right',
        buttons: [{
            formBind: true,
            disabled: true,
            text: '確定',
            handler: function () {
                var form = this.up('form').getForm();
                if (form.isValid()) {
                    var showcode= Ext.getCmp('showcode').getValue();
                    var checkcode= Ext.getCmp('check_code').getValue();
                    if (checkcode == showcode) {
                        if (msg == "請移交主管處理")
                        {
                            document.location.href = "/WareHouse/MarkTally";
                        }
                        if (msg == "請移交主管處理!") {
                            LoadWorkInfo(Ext.getCmp("assg_id").getValue());
                        }
                        codeWin.close();
                        return true;
                    }
                    else {
                        return false;
                    }
                }
            }
        }]
    });
    var codeWin = Ext.create('Ext.window.Window', {
        iconCls: 'icon-user-edit',
        id: 'codeWin',
        width: 300,
        height:200,
        layout: 'fit',
        items: [codeFrm],
        constrain: true,
        closeAction: 'destroy',
        modal: true,
        resizable: false,
        bodyStyle: 'padding:5px 5px 5px 5px',
        closable: false,
       // tools: [
       //{
       //    type: 'close',
       //    qtip: '是否關閉',
       //    handler: function (event, toolEl, panel) {
       //        Ext.MessageBox.confirm(CONFIRM, IS_CLOSEFORM, function (btn) {
       //            if (btn == "yes") {
       //                Ext.getCmp('codeWin').destroy();
       //            }
       //            else {
       //                return false;
       //            }
       //        });
       //    }
       //}],
        listeners: {
            'show': function () {
                Ext.getCmp('info').setValue(msg);
            }
        }
        
    });
    codeWin.show();
}
//返回四位隨機整數驗證碼
function returnCode()
{
    var rnd = "";
    for (var i = 0; i < 4;i++)
    {
        rnd += Math.floor(Math.random()*10);
    }
    return rnd;
}