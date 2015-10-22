Ext.Loader.setConfig({ enabled: true });
Ext.Loader.setPath('Ext.ux', '/Scripts/Ext4.0/ux');
Ext.require([
    'Ext.form.Panel',
    'Ext.ux.form.MultiSelect',
    'Ext.ux.form.ItemSelector'
]);
var excels = ['xls', 'xlsx']; //['xls', 'xlsx'];

Ext.apply(Ext.form.field.VTypes, {
    excelFilter: function (val, field) {
        var type = val.split('.')[val.split('.').length - 1].toLocaleLowerCase();
        for (var i = 0; i < excels.length; i++) {
            if (excels[i] == type) {
                return true;
            }
        }
        return false;
    },
    excelFilterText: FILE_TYPE_WRONG

});


Ext.onReady(function () {
    var exportTab = Ext.create('Ext.form.Panel', {
        layout: 'anchor',
        renderTo: Ext.getBody(),
        width: document.documentElement.clientWidth,
        height: document.documentElement.clientHeight,
        url: '/Vendor/ImportVendorBank',
        border: false,
        plain: true,
        bodyPadding: 20,
        id: 'Import',
        defaults: { anchor: "95%", msgTarget: "side" },
        items: [
              {
                  xtype: 'displayfield',
                  value: '<b>提示:</b><br/><span style="font-size:15px;">1.請匯入格式為xls、xlsx的excel文件<br/>2.首行為欄位名,不予匯入<br/>3.匯入若有異常,請重新核對資料并匯入!</span>'
              },
               {
                   xtype: 'fieldcontainer',
                   combineErrors: false,
                   layout: 'hbox',
                   margin: '10 0 0 0 ',
                   items: [
            {
                xtype: 'filefield',
                name: 'importFile',
                id: 'importFile',
                buttonText: '瀏覽..',
                emptyText: '選擇欲匯入之excel',
                fieldLabel: '匯入excel檔案',
                width: 300,
                submitValue: true,
                fileUpload: true,
                allowBlank: false,
                vtype: 'excelFilter'
            },
             {
                 xtype: 'button',
                 margin: '0 0 0 20',
                 text: "確認匯入",
                 handler: function () {
                     var form = Ext.getCmp('Import').getForm();
                     if (form.isValid()) {
                         form.submit({
                             waitMsg: FILE_UPLOADING,
                             waitTitle: WAIT_TITLE,
                             success: function (form, action) {
                                 var result = Ext.decode(action.response.responseText);
                                 if (result.success) {
                                     if (result.error == "0") {
                                         Ext.Msg.alert(INFORMATION, "資料匯入成功！");

                                     }
                                     else {
                                         if (result.error.split(',').length > 5) {
                                             Ext.Msg.alert(INFORMATION, "代碼: " + result.error.substring(0, 39) + ",……異常,<br/>資料匯入失敗！");
                                         }
                                         else {
                                             Ext.Msg.alert(INFORMATION, "代碼: " + result.error + " 異常,<br/>資料匯入失敗！");
                                         }
                                     }
                                     VendorBankStore.load();
                                     importWin.close();
                                 } else {
                                     Ext.Msg.alert(INFORMATION, "資料匯入失敗,請檢查文件格式！");
                                     Ext.getCmp('sure').setDisabled(false);
                                 }

                             },
                             failure: function (form, action) {
                                 var result = Ext.decode(action.response.responseText);
                                 if (result.error != "") {
                                     Ext.Msg.alert(INFORMATION, result.error);
                                 }
                                 else {
                                     Ext.Msg.alert(INFORMATION, "資料匯入失敗,請聯繫IT人員！");
                                 }
                                 Ext.getCmp('sure').setDisabled(false);
                             }
                         });

                     }
                 }
             }
                   ]
               }
        ]
    });

    Ext.create('Ext.container.Viewport', {
        layout: 'fit',
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


