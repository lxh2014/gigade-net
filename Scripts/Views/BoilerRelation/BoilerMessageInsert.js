Ext.onReady(function () {
    var exportTab = Ext.create('Ext.form.Panel', {
        layout: 'anchor',
        width: 600,
        url: '/BoilerRelation/InsertBoilerMessage',
        defaults: {
            labelWidth: 150,
            width: 400,
            margin: '20 10 0 20'
        },
        border: false,
        plain: true,
        id: 'InvoiceCheck',
        items: [
        {   xtype: 'filefield',
            name: 'ImportFileMsg',
            id: 'ImportFileMsg',
            fieldLabel: '匯入安康內鍋型號對照表',
            msgTarget: 'side',
            buttonText: '瀏覽..',
            submitValue: true,
            allowBlank: false,
            fileUpload: true,
            validator:
                function (value) {
                    var type = value.split('.');
                    if (type[type.length - 1] == 'xls' || type[type.length - 1] == 'xlsx') {
                        return true;
                    }
                    else {
                        return '上傳文件類型不正確！';
                    }
                }
        }
        ,
        {
            xtype: 'displayfield',
            value: '<a href="../../Template/Boiler/型號對照表for吉甲地.xls">' + "下載範例" + '</a>'
        }
        ],
        buttonAlign: 'right',
        buttons: [{
            text: '確定匯入',
            formBind: true,
            disabled: true,
            handler: function () {
                var form = this.up('form').getForm();
                if (form.isValid()) {
                    form.submit({
                        params: {
                            ImportFileMsg: Ext.htmlEncode(Ext.getCmp('ImportFileMsg').getValue())
                        },
                        success: function (form, action) {
                            var result = Ext.decode(action.response.responseText);
                            if (result.success) {
                               var files = result.msg;//文件信息
                               var total = result.total;//匯入數據條數
                               if (parseInt(total) > 0)
                               {
                                   Ext.Msg.alert("提示信息", files + ",共匯入數據" + total + "條");
                               }
                               else
                               {
                                   Ext.Msg.alert("提示信息",files);
                               }
                            }
                            else {
                                Ext.Msg.alert(INFORMATION, FAILURE);
                            }
                        },
                        failure: function (form, action) {
                            var result = Ext.decode(action.response.responseText);
                            Ext.Msg.alert("提示信息",result.msg);
                        }
                    });
                }
            }
        }]
   
    });
    var hpanel = Ext.create('Ext.form.Panel', {
        bodyStyle: "padding:5px;background:#87CEEB",
        border: false,
        html: "注意事項：<br/>1.安康內鍋型號對照表格式請使用UTF-8編碼，否則無法匯入。<br/>2.安康內鍋型號對照表，一列一筆資料，若資料有異常，系統將中斷。<br/>3.安康內鍋型號對照表,'對應安康內鍋型號'欄位,'外鍋型號'欄位,'內鍋型號'欄位填寫時要嚴格按照下載樣本。<br/>4.安康內鍋型號對照表欄位結構不能夠變動。"
    });
    Ext.create('Ext.Viewport', {
        layout: 'anchor',
        items: [exportTab, hpanel],
        autoScroll: true
    });
});