editFunction = function (row, store) {
    var po_TypeStore = Ext.create('Ext.data.Store', {
        fields: ['txt', 'value'],
        data: [
            { "txt": "7", "value": "寄倉品" },
            { "txt": "8", "value": "調度品" }
        ]
    });
    var IpoFrm = Ext.create('Ext.form.Panel', {
        id: 'IpoFrm',
        frame: true,
        plain: true,
        constrain: true,
        defaultType: 'textfield',
        autoScroll: true,
        layout: 'anchor',
        labelWidth: 45,
        url: '/WareHouse/IpoAddOrEdit',
        defaults: { anchor: "99%" },
        items: [
        {
            xtype: 'textfield',
            id: 'row_id',
            name: 'row_id',
            hidden: true,
            allowBlank: true,
            submitValue: true
        },
        {
            xtype: 'textfield',
            id: 'po_id',
            name: 'po_id',
            fieldLabel: '採購單編號',
            allowBlank: false,
            submitValue: true
        },
         {
             xtype: 'textfield',
             id: 'vend_id',
             name: 'vend_id',
             fieldLabel: '廠商代號',
             allowBlank: false,
             submitValue: true
         },
          {
              xtype: 'textfield',
              id: 'buyer',
              name: 'buyer',
              maxLength: 18,
              fieldLabel: '採購員編號',
              allowBlank: false,
              submitValue: true
          },
           {
               xtype: 'combobox',
               name: 'po_type',
               id: 'po_type',
               editable: false,
               fieldLabel: "採購單別",
               store: PoTypeStore,
               queryMode: 'local',
               submitValue: true,
               displayField: 'parameterName',
               valueField: 'ParameterCode',
               typeAhead: true,
               forceSelection: false,
               value: 332,
               listeners: {
                   select:function()
                   {
                       Ext.getCmp('po_type_desc').setValue(Ext.getCmp('po_type').getRawValue());
                   }
               }
           },
          {
              xtype: 'textfield',
              id: 'po_type_desc',
              name: 'po_type_desc',
              fieldLabel: '採購單別描述',
              disabled: true,
              editable: false,
              hidden:false,
              submitValue: true
          },
          {
              xtype: 'datefield',
              id: 'sched_rcpt_dt',
              name: 'sched_rcpt_dt',
              fieldLabel: '預計到貨日期',
              format: 'Y/m/d',
              allowBlank: false,
              submitValue: true
          },
          {
              xtype: 'textfield',
              id: 'msg1',
              name: 'msg1',
              fieldLabel: '備註一',
              allowBlank: true,
              submitValue: true,
              maxLength: 35
          },
           {
               xtype: 'textfield',
               id: 'msg2',
               name: 'msg2',
               fieldLabel: '備註二',
               allowBlank: true,
               submitValue: true,
               maxLength:35
           },
           {
               xtype: 'textfield',
               id: 'msg3',
               name: 'msg3',
               fieldLabel: '備註三',
               allowBlank: true,
               submitValue: true,
               maxLength: 35
           }
        ],
        buttons: [
        {
            xtype: 'button',
            vtype: 'submit',
            formBind: true,
            disabled: true,
            text: '保存',
            handler: function () {
                var form = this.up('form').getForm();
                if (form.isValid()) {
                        form.submit({
                            params: {
                                row_id: Ext.htmlEncode(Ext.getCmp('row_id').getValue()),
                                po_id: Ext.htmlEncode(Ext.getCmp('po_id').getValue()),
                                vend_id: Ext.htmlEncode(Ext.getCmp('vend_id').getValue()),
                                buyer: Ext.htmlEncode(Ext.getCmp('buyer').getValue()),
                                po_type: Ext.htmlEncode(Ext.getCmp('po_type').getValue()),
                                po_type_desc: Ext.htmlEncode(Ext.getCmp('po_type_desc').getValue()),
                                sched_rcpt_dt: Ext.htmlEncode(Ext.getCmp('sched_rcpt_dt').getValue()),
                                msg1: Ext.htmlEncode(Ext.getCmp('msg1').getValue()),
                                msg2: Ext.htmlEncode(Ext.getCmp('msg2').getValue()),
                                msg3: Ext.htmlEncode(Ext.getCmp('msg3').getValue())
                            },
                            success: function (form, action) {
                                var result = Ext.decode(action.response.responseText);
                                if (result.success) {
                                    if (result.msg != undefined) {
                                        Ext.Msg.alert(INFORMATION, result.msg);
                                    } else {
                                        Ext.Msg.alert(INFORMATION, "操作成功!");
                                        IpoStore.load();
                                        editDisKeyWordsWin.close();
                                    }
                                }
                                else {
                                    Ext.Msg.alert(INFORMATION, FAILURE);
                                }
                            },
                            failure: function () {

                                Ext.Msg.alert(INFORMATION, FAILURE);
                            }
                        });
                }
            }
        }
        ]
    });
    var editDisKeyWordsWin = Ext.create('Ext.window.Window', {
        title: '新增/編輯採購單單頭',
        iconCls: 'icon-user-edit',
        id: 'editDisKeyWordsWin',
        width: 420,
        height: 420,
        layout: 'fit',
        items: [IpoFrm],
        constrain: true,
        closeAction: 'destroy',
        modal: true,
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
                        Ext.getCmp('editDisKeyWordsWin').destroy();
                    }
                    else {
                        return false;
                    }
                });
            }
        }],
        listeners: {
            'show': function () {
                if (row != null) {
                    IpoFrm.getForm().loadRecord(row);
                    Ext.getCmp('po_id').setDisabled(true);
                }
            }
        }
    });
    editDisKeyWordsWin.show();
}