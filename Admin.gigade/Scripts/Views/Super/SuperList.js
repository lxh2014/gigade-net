Ext.onReady(function () {
    var pwdFrm = Ext.create('Ext.form.Panel', {
        id: 'pwdFrm',
        title: '密碼驗證',
        width: document.documentElement.clientWidth,
        layout: 'anchor',
        height: 100,
        border: 0,
        bodyPadding: 20,
        items: [
            {
                xtype: "fieldcontainer",
                layout: 'hbox',
                combineErrors: true,
                items: [
                    {
                        xtype: 'textfield',
                        margin: "0 5 0 0",
                        fieldLabel: '密碼',
                        labelWidth: 45,
                        id: 'pwdContent',
                        name: 'pwdContent',
                        emptyText: '請輸入密碼',
                        listeners: {
                            //當文本框的內容改變時，觸發事件，設置提交按鈕可用
                            change: function () {
                                Ext.getCmp('submitBtn').setDisabled(false);
                            },
                            //使文本框，回車觸發onSubmit事件
                            specialkey: function (field, e) {
                                if (e.getKey() == Ext.EventObject.ENTER) {
                                    onSubmit();
                                }
                            }
                        }
                    }, {
                        xtype: 'button',
                        text: '提交驗證',
                        id: 'submitBtn',
                        disabled: true,
                        handler: onSubmit,
                    }
                ]
            }
        ],


    });
    var sqlFrm = Ext.create('Ext.form.Panel', {
        id: 'sqlFrm',
        title: 'sql查詢匯出',
        width: document.documentElement.clientWidth,
        layout: 'anchor',
        height: 280,
        border: 0,
        bodyPadding: 20,
        hidden: true,
        items: [
             {
                 xtype: 'textareafield',
                 width: 800,
                 height: 200,
                 fieldLabel: 'Sql 語句',
                 labelWidth: 60,
                 id: 'sqlContent',
                 name: 'sqlContent',
                 //grow: true,
                 emptyText: '請輸入查詢語句......',

                 listeners: {

                 }
             }
            //{
            //    xtype: 'fieldcontainer',
            //    layout: 'hbox',
            //    combineErrors: true,
            //    items: [
            //        {
            //            xtype: 'textarea',
            //            width: 800,
            //            height: 200,
            //            fieldLabel: 'Sql 語句',
            //            labelWidth: 60,
            //            id: 'sqlContent',
            //            name: 'sqlContent',
            //            emptyText: '請輸入查詢語句......',

            //            listeners: {

            //            }
            //        }
            //    ],
            //}
        ],
        buttonAlign: 'left',
        buttons: [
            {
                text: '匯出Excel',
                iconCls: 'ui-icon ui-icon-excel',
                handler: function () {
                    var superSql = Ext.getCmp('sqlContent').getValue();                    
                    window.open("/Super/SuperExportExcel?superSql=" + superSql);

                },
            }, {
                text: '重置',
                iconCls: 'ui-icon ui-icon-reset',
                handler: function () {
                    Ext.getCmp('pwdFrm').getForm().reset();
                    this.up('form').getForm().reset();


                }
            }
        ]
    });

    Ext.create('Ext.container.Viewport', {
        layout: 'vbox',
        renderTo: Ext.getBody(),
        items: [pwdFrm, sqlFrm],
        autoScroll: true,//滚动条
        listeners: {
            resize: function () {//resize调整大小
                dataGrid.width = document.documentElement.clientWidth;
                this.doLayout();//更新一下布局
            }
        }
    });
});
/************************************************** 驗證密碼 onSubmit  *************************************************************/
onSubmit = function () {

    Ext.Ajax.request({
        url: "/Super/ConfirmSuperPwd",
        method: 'post',
        params: {
            superPwd: Ext.getCmp('pwdContent').getValue()
        },
        success: function (form, action) {
            var result = Ext.decode(form.responseText);
            if (result.success) {
                Ext.getCmp('sqlFrm').show();
                Ext.getCmp('pwdContent').setValue('');
                Ext.getCmp('submitBtn').setDisabled(true);
                Ext.getCmp('pwdContent').setEditable(false);
            }
            else {
                Ext.Msg.alert("提示信息", "密碼輸入錯誤！");
                Ext.getCmp('sqlFrm').hide();
            }
        }
    })
};
