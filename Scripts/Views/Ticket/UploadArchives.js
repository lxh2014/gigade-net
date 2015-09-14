
Ext.onReady(function () {
    var exportTab = Ext.create('Ext.form.Panel', {
        layout: 'anchor',
        width: 600,
        url: '/Ticket/GetUploadArchives',
        //standardSubmit: true,
        margin: '0 10 0 0',
        defaults: {
            labelWidth: 150,
            width: 400,
            margin: '20 10 0 20'
        },
        border: false,
        plain: true,
        id: 'OrderMasterImport',
        items: [
            {
                xtype: 'panel',
                bodyStyle: "padding:5px;background:#EEEEEE",//background:#87CEEB
                border: false,
                html: "注意事項：<br/>會計，供應商資料上傳"
               // html: "注意事項：<br/>1.檔案為.xls<br/>2.欄位：付款單號、銀行入帳日期、入賬金額、手續費、備註。<br/>3.當檔案中存在異常時,將不會處理異常數據,且其它數據會繼續匯入.<br/>4.<a href='javascript:void(0);' onclick='ShowMuBan()'>點擊下載匯入模板</a>"
            },
            {
                xtype: 'filefield',
                name: 'ImportFileMsg',
                id: 'ImportFileMsg',
                fieldLabel: '上傳檔案',
                msgTarget: 'side',
                buttonText: '瀏覽..',
                submitValue: true,
                allowBlank: false,
                fileUpload: true
                //validator: function (value) {
                //    var type = value.split('.');
                //    if (type[type.length - 1] == 'xls' || type[type.length - 1] == 'xlsx') {
                //        return true;
                //    } else {
                //        return '上傳文件類型不正確！';
                //    }
                //}
            }
        ],
        buttonAlign: 'right',
        buttons: [{
            text: '確定上傳',
            formBind: true,
            disabled: true,
            handler: function () {
                var form = this.up('form').getForm();
                if (form.isValid()) {
                    var myMask = new Ext.LoadMask(Ext.getBody(), { msg: 'Loading...' });
                    myMask.show();
                    form.submit({
                        params: {
                            ImportFileMsg: Ext.htmlEncode(Ext.getCmp('ImportFileMsg').getValue())
                        },
                        timeout: 900000,
                        success: function (form, action) {
                            var result = Ext.decode(action.response.responseText);
                        
                            myMask.hide();
                            if (result.success=="true") {
                                Ext.Msg.alert("提示信息", "上傳成功!");
                            }
                            else
                            {
                                Ext.Msg.alert("提示信息",result.msg);
                            } 
                        },
                        failure: function (form, action) {
                            myMask.hide();
                            Ext.Msg.alert("提示信息", result.msg);
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
