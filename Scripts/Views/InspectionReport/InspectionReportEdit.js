

editFunction = function (row, store) {
    var comType = false;
    var isGoOn = true;
    Ext.define('GIGADE.CertificateModelType1', {
        extend: 'Ext.data.Model',
        fields: [
            { name: 'certificate_categoryname', type: 'string' },
            { name: 'frowID', type: 'int' }
        ]
    })
    var CertificateStoreType1 = Ext.create('Ext.data.Store', {
        model: 'GIGADE.CertificateModelType1',
        autoLoad: true,
        proxy: {
            type: 'ajax',
            url: "/InspectionReport/GetType1Group",
            actionMethods: 'post',
            reader: {
                type: 'json',
                root: 'data'
            }
        }
    });
    Ext.define('GIGADE.CertificateModelType2', {
        extend: 'Ext.data.Model',
        fields: [
            { name: 'certificate_categoryname', type: 'string' },
            { name: 'frowID', type: 'int' }
        ]
    })
    var CertificateStoreType2 = Ext.create('Ext.data.Store', {
        model: 'GIGADE.CertificateModelType2',
        autoLoad: false,
        proxy: {
            type: 'ajax',
            url: "/InspectionReport/GetType2Group",
            actionMethods: 'post',
            reader: {
                type: 'json',
                root: 'data'
            }

        }
    });

    CertificateStoreType2.on('beforeload', function () {
        Ext.apply(CertificateStoreType2.proxy.extraParams,
            {
                ROWID: Ext.getCmp('certificate_type1').getValue(),
            });
    });
    var editFrm = Ext.create('Ext.form.Panel', {
        id: 'editFrm',
        frame: true,
        autoScroll: true,
        plain: true,
        layout: 'anchor',
        url: '/InspectionReport/SaveInspectionRe',
        labelWidth: 45,
        defaults: { anchor: "95%", msgTarget: "side" },
        items: [
            {
                xtype: 'displayfield',
                fieldLabel: "流水號",
                id: 'rowID',
                name: 'rowID',
                hidden: true,
            },
              {
                  xtype: 'textfield',
                  fieldLabel: "商品編號",
                  id: 'product_id',
                  name: 'product_id',
                  allowBlank: false,
                  regex:/^[0-9]*$/,
                  listeners: {
                      blur: function () {
                          var id = Ext.getCmp('product_id').getValue();
                          var regex =/^[0-9]*$/;
                          if (regex.test(id)) {
                              Ext.Ajax.request({
                                  url: "/Vote/GetProductName",
                                  method: 'post',
                                  type: 'text',
                                  params: {
                                      id: Ext.getCmp('product_id').getValue()
                                  },
                                  success: function (form, action) {
                                      var result = Ext.decode(form.responseText);
                                      if (result.success) {
                                          var msg = result.msg;
                                          if (msg == 0) {
                                              Ext.getCmp("product_name").setValue("沒有該商品信息！");
                                          }
                                          else {
                                              Ext.getCmp("product_name").setValue(msg);
                                          }
                                      }
                                  }
                              });
                          }
                          else {
                              Ext.getCmp("product_name").setValue("沒有該商品信息！");
                          }
                          if (Ext.getCmp("product_name").getValue != "沒有該商品信息！") {
                              Ext.Ajax.request({
                                  url: "/InspectionReport/GetBrandID",
                                  method: 'post',
                                  type: 'text',
                                  params: {
                                      product_id: Ext.getCmp('product_id').getValue()
                                  },
                                  success: function (form, action) {
                                      var result = Ext.decode(form.responseText);
                                      if (result.success) {
                                          Ext.getCmp("brand_id").setValue(result.brand_id);
                                      }
                                      else {
                                          Ext.getCmp("brand_id").setValue("無此品牌");
                                      }
                                  }
                              });
                          }
                      },
                  }
              },
              {
                  xtype: 'displayfield',
                  fieldLabel: "商品名稱",
                  name: 'product_name',
                  id: 'product_name',
              },
              {
                  xtype: 'displayfield',
                  fieldLabel: "品牌編號",
                  name: 'brand_id',
                  id: 'brand_id',
                  hidden:true,
              },
        {
            xtype: 'combo',
            fieldLabel: "證書大類",
            store: CertificateStoreType1,
            lastQuery:'',
            valueField: 'frowID',
            editable:false,
            displayField: 'certificate_categoryname',
            id: 'certificate_type1',
            name: 'certificate_type1',
            allowBlank: false,
            listeners: {
                select: function (combo, record) {
                    var m = Ext.getCmp("certificate_type2");
                    m.clearValue();
                    CertificateStoreType2.removeAll();
                    comType = true;
                }
            },
        }, {
            xtype: 'combo',
            fieldLabel: "證書小類",
            store: CertificateStoreType2,
            editable: false,
            id: 'certificate_type2',
            name: 'certificate_type2',
            valueField: 'frowID',
            displayField: 'certificate_categoryname',
            allowBlank: false,
            listeners: {
                beforequery: function (qe) {
                    if (comType) {
                        delete qe.combo.lastQuery;
                        CertificateStoreType2.load({
                            params: {
                                ROWID: Ext.getCmp('certificate_type1').getValue(),
                            }
                        });
                        comType = false;
                    }
                }
            }
        },
            {
                xtype: 'numberfield',
                id: 'old_sort',
                name: 'old_sort',
                fieldLabel: '排序',
                allowDecimals: false,
                minValue: 0,
                maxValue: 999999999,
                hidden:true,
            },
        {
            xtype: 'numberfield',
            id: 'sort',
            name: 'sort',
            fieldLabel: '排序',
            allowBlank: false,
            allowDecimals: false,
           // editable: false,
            minValue: 0,
            maxValue: 999999999,
          
        },
        {
            xtype: 'datetimefield',
            fieldLabel: "有效期限",
            format: 'Y-m-d H:i:s',
            time: { hour: 23, min: 59, sec: 59 },
            id: 'certificate_expdate',
            name: 'certificate_expdate',

            editable: false,
            allowBlank: false,
        }, {
            xtype: 'textarea',
            fieldLabel: "說明(最大50字)",
            id: 'certificate_desc',
            maxLength:50,
            name: 'certificate_desc',
        }, {
            xtype: 'filefield',
            fieldLabel: "上傳圖片",
            buttonText: '選擇檔案',
            id: 'certificate_filename',
            name: 'certificate_filename',
            editable:true,
            anchor: '100%',
            allowBlank: false,
            validator:
            function (value) {
                if (value != '') {
                    var type = value.split('.');
                    if (type[type.length - 1] == 'jpg'||type[type.length - 1] == 'JPG') {
                        return true;
                    }
                    else {
                        return '上傳文件類型不正確！';
                    }
                }
            },
        }
        ],
        buttons: [{
            text: '保存',
            id:'save',
            formBind: true,
            disabled: true,
            handler: function () {
                if (Ext.getCmp("product_name").getValue() == "沒有該商品信息！") {
                    Ext.getCmp("product_id").setValue("");
                    return;
                }
                if (Ext.getCmp("certificate_filename").getValue() == "")
                {
                    Ext.Msg.alert("提示信息", "還未選擇檔案！");
                    return;
                }
                
                var form = this.up('form').getForm();
                var myMask = new Ext.LoadMask(Ext.getBody(), { msg: "Please wait..." });
                myMask.show();
                if (form.isValid()) {
                    form.submit({
                        params: {
                            rowID: Ext.htmlEncode(Ext.getCmp('rowID').getValue()),
                            product_id: Ext.htmlEncode(Ext.getCmp('product_id').getValue()),
                            brand_id: Ext.htmlEncode(Ext.getCmp('brand_id').getValue()),
                            certificate_type1: Ext.htmlEncode(Ext.getCmp('certificate_type1').getValue()),
                            certificate_type2: Ext.htmlEncode(Ext.getCmp('certificate_type2').getValue()),
                            sort: Ext.htmlEncode(Ext.getCmp('sort').getValue()),
                            certificate_expdate: Ext.htmlEncode(Ext.getCmp('certificate_expdate').getValue()),
                            certificate_desc: Ext.htmlEncode(Ext.getCmp('certificate_desc').getValue()),
                            certificate_filename: Ext.htmlEncode(Ext.getCmp('certificate_filename').getValue()),
                            old_sort: Ext.htmlEncode(Ext.getCmp('old_sort').getValue()),
                        },
                        success: function (form, action) {
                            myMask.hide();
                            if (action.result.success) {
                                myMask.hide();
                                if (action.result.msg == "0") {
                                    Ext.Msg.alert("提示信息", "保存成功！");
                                    editWin.close();
                                    store.load();
                                }
                                else if (action.result.msg == "1") {
                                    myMask.hide();
                                    Ext.Msg.alert("提示信息", "保存失敗！");
                                    editWin.close();
                                }
                                else if (action.result.msg == "2") {
                                    myMask.hide();
                                    Ext.Msg.alert("提示信息", "數據重複,請重新選擇！");
                                    Ext.getCmp('product_id').setValue('');
                                    Ext.getCmp('product_name').setValue('');
                                    Ext.getCmp('brand_id').setValue('');
                                    Ext.getCmp('certificate_type1').setValue('');
                                    Ext.getCmp('certificate_type2').setValue('');
                                }
                                else if (action.result.msg == "3") {
                                    myMask.hide();
                                    Ext.Msg.alert("提示信息", "所選檔案名稱不正確，請重新選擇！");
                                    Ext.getCmp('certificate_filename').setRawValue(row.data.certificate_filename);
                                }
                                else if (action.result.msg == "4") {
                              
                                    myMask.hide();
                                    if (row) {
                                        Ext.getCmp('certificate_filename').setRawValue(row.data.certificate_filename);
                                    }
                                    Ext.Msg.alert("提示信息", "排序重複 ！");
                                }
                                else {
                                    myMask.hide();
                                    Ext.Msg.alert("提示信息", action.result.msg);
                                }
                            }
                            else {
                                myMask.hide();
                                Ext.Msg.alert("提示信息", "保存失敗！");
                                editWin.close();
                               // store.load();
                            }
                         

                        },
                        failure: function (form, action) {
                            myMask.hide();
                            Ext.Msg.alert("提示信息", "保存失敗！");
                            editWin.close();
                            //store.load();
                        }
                    });
                }
            }

        }]
    });

    var editWin = Ext.create('Ext.window.Window', {
        id: 'editWin',
        title: '新增/編輯檢驗報告',
        iconCls: 'icon-user-add',
        width: 455,
        height: 355,
        layout: 'fit',
        constrain: true,
        closeAction: 'destroy',
        modal: true,
        closable: false,
        items: [editFrm],
        tools: [
    {
        type: 'close',
        qtip: '是否關閉',
        handler: function (event, toolEl, panel) {
            Ext.MessageBox.confirm("確認信息", "是否關閉窗口", function (btn) {
                if (btn == "yes") {
                    Ext.getCmp('editWin').destroy();
                }
                else {
                    return false;
                }
            });
        }
    }
        ],
        listeners: {
            'show': function () {
                if (row) {
                    editFrm.getForm().loadRecord(row);
                    
                    Ext.getCmp("old_sort").setRawValue(row.data.sort);
                    Ext.getCmp("product_id").setDisabled(true);
                    Ext.getCmp("certificate_type1").setDisabled(true);
                    Ext.getCmp("certificate_type2").setDisabled(true);
                    Ext.getCmp("certificate_filename").setRawValue(row.data.certificate_filename);
                    CertificateStoreType2.load({
                        params: {
                            ROWID: Ext.getCmp('certificate_type1').getValue(),
                        }
                    });
                }
            }

        },

    });
    editWin.show();
    function IsExit()
    {
        Ext.Ajax.request({
            url: '/InspectionReport/IsExist',
            params: {
                product_id: Ext.htmlEncode(Ext.getCmp('product_id').getValue()),
                brand_id: Ext.htmlEncode(Ext.getCmp('brand_id').getValue()),
                certificate_type1: Ext.htmlEncode(Ext.getCmp('certificate_type1').getValue()),
                certificate_type2: Ext.htmlEncode(Ext.getCmp('certificate_type2').getValue()),
            },
            success: function (form, action) {
                var result = Ext.decode(form.responseText);
                if (result.success) {
                    isGoOn = false;
                }
                else {
                    isGoOn = true;
                }
            }
        });
        return isGoOn;
    }
};