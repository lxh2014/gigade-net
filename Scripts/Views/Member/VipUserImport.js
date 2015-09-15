Ext.Loader.setConfig({ enabled: true });
Ext.Loader.setPath('Ext.ux', '/Scripts/Ext4.0/ux');
Ext.require([
    'Ext.form.Panel',
    'Ext.ux.form.MultiSelect',
    'Ext.ux.form.ItemSelector'
]);

Ext.onReady(function () {
    var exportTab = Ext.create('Ext.form.Panel', {
        layout: 'anchor',
        renderTo: Ext.getBody(),
        width: 400,
        height: 420,
        url: '/Member/ImportCsv',
        border: false,
        plain: true,
        defaultType: 'displayfield',
        bodyPadding: 20,
        labelWidth: 45,
        id: 'Import',
        defaults: { anchor: "95%", msgTarget: "side" },
        items: [
        {
            xtype: 'displayfield',
            id: 'group_id',
            colName: 'group_id',
            name: 'group_id',
            fieldLabel: '名單編號',
            value: document.getElementById("groupid").value

        },
        {
            xtype: 'displayfield',
            id: 'group_name',
            colName: 'group_name',
            name: 'group_name',
            fieldLabel: '名單名稱',
            value: document.getElementById("groupname").value
        },
        {
            xtype: 'filefield',
            name: 'ImportCsvFile',
            id: 'ImportCsvFile',
            fieldLabel: '匯入csv檔案',
            msgTarget: 'side',
            buttonText: '瀏覽..',
            submitValue: true,
            validator:
            function (value) {
                var type = value.split('.');
                if (type[type.length - 1] == 'csv') {
                    return true;
                }
                else {
                    return '上傳文件類型不正確！';
                }
            },
            width: 300,
            allowBlank: false,
            fileUpload: true
        },
        {
            border: 0,
            bodyStyle: 'padding:5px 5px 5px 5px',
            xtype: 'displayfield',
            value: '<a href="../../../Template/VipUsers/vipgroupuser.csv">範例下載</a>'
        },
        {
            xtype: 'displayfield',
            id: 'carethings',
            name: 'carethings',
            fieldLabel: '注意事項',
            value: document.getElementById('care').value
        }],
        buttonAlign: 'right',
        buttons: [{
            text: '上傳',
            id: 'import',
            formBind: true,
            disabled: true,
            handler: function () {
                var form = this.up('form').getForm();
                if (form.isValid()) {
                    form.submit({
                        params: {
                            group_id: Ext.htmlEncode(Ext.getCmp('group_id').getValue()),
                            group_name: Ext.htmlEncode(Ext.getCmp('group_name').getValue()),
                            check_iden: document.getElementById('check').value,
                            file: Ext.htmlEncode(Ext.getCmp('ImportCsvFile').getValue())
                        },
                        success: function (form, action) {
                            var result = Ext.decode(action.response.responseText);
                            if (result.success) {
                                Ext.Msg.alert(INFORMATION, result.msg);
                            }
                            else {
                                Ext.Msg.alert(INFORMATION, result.msg);
                            }
                        },
                        failure: function (form, action) {
                            var result = Ext.decode(action.response.responseText);
                            Ext.Msg.alert(INFORMATION, result.msg);
                        }
                    });
                }
            }
        },
        {
            iconCls: 'icon_rewind',
            text: RETURN,
            scale: 'small',
            style: { marginRight: '10px' },
            handler: function () {
                window.location.href = '/Member/VipUserGroupList';
            }
        }]
    });

    Ext.create('Ext.container.Viewport', {
        layout: 'fit',
        items: [exportTab],
        renderTo: Ext.getBody(),
        autoScroll: true,
        listeners: {
            resize: function () {
                // exportTab.width = document.documentElement.clientWidth;
                this.doLayout();
            }
        }
    });
});
