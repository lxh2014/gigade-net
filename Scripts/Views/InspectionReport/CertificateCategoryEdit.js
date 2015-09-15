function editFunction(row, store) {

    Ext.define('GIGADE.CertificateModel', {
        extend: 'Ext.data.Model',
        fields: [
            { name: 'certificate_categoryname', type: 'string' },
            { name: 'frowID', type: 'string' }
        ]
    })

    var CertificateStore = Ext.create('Ext.data.Store', {
        model: 'GIGADE.CertificateModel',
        autoLoad: true,
        proxy: {
            type: 'ajax',
            url: "/InspectionReport/GetGroup",
            actionMethods: 'post',
            reader: {
                type: 'json',
                root: 'data'
            }

        }
    });






    var Certificate_editFrm = Ext.create('Ext.form.Panel', {
        id: 'Certificate_editFrm',
        frame: true,
        plain: true,
        constrain: true,
        autoScroll: true,
        layout: 'anchor',
        url: '/InspectionReport/InspectionReportSave',
        defaults: { anchor: "95%", msgTarget: "side", labelWidth: 80 },
        items: [
            {
                xtype: 'textfield',
                id: 'rowID',
                name: 'rowID',
                fieldLabel: '證書小類id',
                hidden:true
            },
            {
                xtype: 'textfield',
                id: 'frowID',
                name: 'frowID',
                fieldLabel: '證書大類id',
                hidden: true
            },
                {
                   xtype: 'combobox',
                   store: CertificateStore,
                   displayField: 'certificate_categoryname',
                   valueField: 'frowID',
                   lastQuery: '',
                   fieldLabel: "證書-大類名稱",
                   labelWidth: 100,
                   listWidth:100,
                   allowBlank: false,
                   queryMode: 'local',
                   id: 'frowname',
                   Name: 'frowname',
                   regex: /[a-z]|[A-Z]|[\u4e00-\u9fa5]/,
                   regexText: '名稱格式不正確',
                   typeAhead: true,
                   value: '',
                   listeners: {
                       'select': function () {
                           var id = Ext.getCmp('frowname').getValue().trim();
                           var numtest = /^[0-9]*$/;
                           if (numtest.test(id)) {
                               Ext.Ajax.request({
                                   url: "/InspectionReport/GetBigCode",
                                   method: 'post',
                                   type: 'text',
                                   params: {
                                       id: id
                                   },
                                   success: function (form, action) {
                                       var result = Ext.decode(form.responseText);
                                       if (result.success) {
                                           msg = result.msg;
                                           Ext.getCmp("frowID").setValue(result.fid);
                                           Ext.getCmp("certificate_categorycode").setValue(msg);
                                       }

                                   }
                               });
                           }
                       }

                   }
               },
               {
                   xtype: 'textfield',
                   fieldLabel: "證書-大類CODE",
                   labelWidth: 100,
                   Width: 200,
                   regex: /^[0-9a-zA-Z]+$/,
                   regexText: '大類CODE格式不正確',
                   allowBlank: false,
                   id: 'certificate_categorycode',
                   name: 'certificate_categorycode'
                  
               } ,{
                   xtype: 'textfield',
                   fieldLabel: "證書-小類名稱",
                   labelWidth: 100,
                   allowBlank: false,
                   Width: 200,
                   id: 'certificate_category_childname',
                   name: 'certificate_category_childname'
               },
                {
                   xtype: 'textfield',
                   fieldLabel: "證書-小類CODE",
                   labelWidth: 100,
                   regex: /^[0-9a-zA-Z]+$/,
                   regexText: '小類CODE格式不正確',
                   allowBlank: false,
                   Width:200,
                   id: 'certificate_category_childcode',
                   name: 'certificate_category_childcode'
                },
        ],
        buttons: [{
            text: SAVE,
            formBind: true,
            disabled: true,
            handler: function () {
                var form = this.up('form').getForm();

                    if (form.isValid()) {
                        form.submit({
                            params: {
                                rowID: Ext.getCmp('rowID').getValue().trim(),
                                frowID: Ext.getCmp('frowID').getValue().trim(),
                                frowname: Ext.getCmp('frowname').getValue() == null ? Ext.getCmp('frowID').getValue().trim() : Ext.getCmp('frowname').getValue(),
                                f_code: Ext.getCmp('certificate_categorycode').getValue().trim(),
                                child_name: Ext.getCmp('certificate_category_childname').getValue().trim(),
                                child_code: Ext.getCmp('certificate_category_childcode').getValue()

                            },
                            success: function (form, action) {
                                var result = Ext.decode(action.response.responseText);
                                if (result.success) {
                                    if (result.type == 1) {
                                        Ext.Msg.alert(INFORMATION, result.msg);
                                    }
                                    else {
                                        Ext.Msg.alert(INFORMATION, result.msg);
                                        Certificate_editWins.close();
                                        CertificateCategoryStore.load();
                                    }
                                }
                            },
                            failure: function () {
                                Ext.Msg.alert(INFORMATION, FAILURE);
                            }
                        });
                    }
                    //else {
                    //    Ext.Msg.alert(INFORMATION, PRODTIP);
                    //}
                
            }
        }]
    });
    var Certificate_editWins = Ext.create('Ext.window.Window', {
        title: "證書類別新增/編輯",
        iconCls: row ? 'icon-user-edit' : 'icon-user-add',
        id: 'Certificate_editWins',
        width: 350,
        height: 250,
        layout: 'fit',
        items: [Certificate_editFrm],
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
                qtip: "關閉",
                handler: function (event, toolEl, panel) {
                    Ext.MessageBox.confirm(CONFIRM, IS_CLOSEFORM, function (btn) {
                        if (btn == "yes") {
                            Ext.getCmp('Certificate_editWins').destroy();
                        } else {
                            return false;
                        }
                    });
                }
            }
        ],
        listeners: {
            'show': function () {
                if (row == null) {
                    Certificate_editFrm.getForm().reset(); //如果是添加的話
                }
                else {
                    Certificate_editFrm.getForm().loadRecord(row); //如果是編輯的話
                    if (row.data.frowID != 0) {
                        Ext.getCmp("frowname").setValue(row.data.frowID);
                    }
                    //Ext.getCmp("frowname").forceSelection = true;
                }

            }
        }
    });
    Certificate_editWins.show();



}