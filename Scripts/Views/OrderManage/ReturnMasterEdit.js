
editFunction = function (row, store) {

    Ext.define('gigade.zipAddress', {
        extend: 'Ext.data.Model',
        fields: [
            { name: "zipcode", type: "string" },
            { name: "zipname", type: "string" }
        ]
    });
    var zipAddressStore = Ext.create('Ext.data.Store', {
        model: 'gigade.zipAddress',
        autoLoad: true,
        proxy: {
            type: 'ajax',
            url: "/VipUserGroup/GetZipStore",
            reader: {
                type: 'json',
                root: 'data'
            }
        }
    });
    //地址Model
    Ext.define('gigade.City', {
        extend: 'Ext.data.Model',
        fields: [
        { name: "middle", type: "string" },
        { name: "middlecode", type: "string" }]
    });

    //郵編Model
    Ext.define('gigade.Zip', {
        extend: 'Ext.data.Model',
        fields: [
        { name: "zipcode", type: "string" },
        { name: "small", type: "string" }]
    });

    //會員地址Store
    var CityStore = Ext.create('Ext.data.Store', {
        model: 'gigade.City',
        autoLoad: true,
        proxy: {
            type: 'ajax',
            url: "/OrderManage/QueryCity",
            reader: {
                type: 'json',
                root: 'items'
            }
        }
    });
    //會員郵編Store
    var ZipStore = Ext.create('Ext.data.Store', {
        autoLoad: false,
        model: 'gigade.Zip',
        proxy: {
            type: 'ajax',
            url: "/OrderManage/QueryZip",
            actionMethods: 'post',
            reader: {
                type: 'json',
                root: 'items'
            }
        }
    });
    //發票處理
    var InvoiceDealStore = Ext.create('Ext.data.Store', {
        fields: ['txt', 'value'],
        data: [
        { "txt": "所有列表", "value": "0" },
        { "txt": "發票作廢", "value": "1" },
        { "txt": "發票退回", "value": "2" },
        { "txt": "未收到發票", "value": "3" },
        { "txt": "折讓單退回", "value": "4" },
        { "txt": "未收到折讓單", "value": "5" }
        ]
    });
    //退貨處理
    var PackageStore = Ext.create('Ext.data.Store', {
        fields: ['txt', 'value'],
        data: [
        { "txt": "所有列表", "value": "0" },
        { "txt": "包裝完整", "value": "1" },
        { "txt": "贈品未回", "value": "2" },
        { "txt": "配件不齊", "value": "3" },
        { "txt": "消耗品拆封", "value": "4" }
        ]
    });


    var editFrm = Ext.create('Ext.form.Panel', {
        id: 'editFrm',
        frame: true,
        plain: true,
        constrain: true,
        defaultType: 'textfield',
        layout: 'anchor',
        labelWidth: 45,
        bodyPadding: 10,
        url: '/OrderManage/SaveOrderReturnMaster',
        defaults: { anchor: "98%", msgTarget: "side" },
        items: [
            {
                xtype: 'displayfield',
                fieldLabel: '退貨單號',
                id: 'return_id',
                name: 'return_id',
                submitValue: true
            },
        {
            xtype: 'displayfield',
            name: 'order_id',
            id: 'order_id',
            submitValue: true,
            fieldLabel: '付款單號',

        },
        {
            xtype: 'displayfield',
            name: 'vendor_name',
            id: 'vendor_name',
            fieldLabel: '出貨商',            
        },
        {
            xtype: 'combobox',
            allowBlank: false,
            value: 0,
            id: 'invoice_deal',
            name: 'invoice_deal',
            store: InvoiceDealStore,
            queryMode: 'local',
            displayField: 'txt',
            valueField: 'value',
            typeAhead: true,
            width: 100,
            forceSelection: false,
            submitValue: true,
            editable: false,
            fieldLabel: "發票處理"
        },
        //{
        //    xtype: 'combobox',
        //    allowBlank: false,
        //    id: 'package',
        //    name: 'package',
        //    value: 0,
        //    store: PackageStore,
        //    queryMode: 'local',
        //    width: 100,
        //    displayField: 'txt',
        //    valueField: 'value',
        //    typeAhead: true,
        //    forceSelection: false,
        //    editable: false,
        //    fieldLabel: "退貨處理"
        //},
        //{
        //    xtype: 'fieldcontainer',
        //    layout: 'hbox',
        //    items: [{
        //        xtype: 'combobox',
        //        valueField: 'zipcode',
        //        editable: false,
        //        displayField: 'zipname',
        //        id: 'return_zip',
        //        name:'return_zip',
        //        queryModel: 'local',
        //        store: zipAddressStore,
        //        fieldLabel: '收貨地址',
        //        allowBlank:false,
        //        emptyText:'請選擇...',
        //    },
        //       {
        //           id: 'return_address',
        //           name: 'return_address',
        //           xtype: 'textfield',
        //           width: 150,
        //           submitValue: true
        //       },
        //    ],         
        //},
        //{
        //    xtype: 'textfield',
        //    name: 'deliver_code',
        //    id: 'deliver_code',
        //    submitValue: true,
        //    fieldLabel: '退貨物流單號'
        //},
        {
            xtype: 'radiogroup',
            id: 'return_statuss',
            name: 'return_statuss',
            fieldLabel: '狀態',
            submitValue: true,
            colName: 'returnstatus',
            defaults: {
                margin: '0 8 0 0'
            },
            columns:3,
            vertical: true,
            items: [
                    {
                        boxLabel: '待歸檔',
                        name: 'returnstatus',
                        id: 'radio1',
                        inputValue: "0",
                        checked: true
                    },
                    //{
                    //    boxLabel: '歸檔',
                    //    name: 'returnstatus',
                    //    id: 'radio2',
                    //    inputValue: "1"
                    //},
                       {
                           boxLabel: '取消退貨',
                           name: 'returnstatus',
                           id: 'radio3',
                           inputValue: "2"
                       }
            ]
        },
          {
              xtype: 'textfield',
              name: 'order_payment',
              id: 'order_payment',
              submitValue: true,
              fieldLabel: '付款方式',
              hidden: true
          },
          {
              xtype: 'textareafield',
              name: 'return_note',
              id: 'return_note',
              submitValue: true,
              fieldLabel: '備註'
          },
          {
              xtype: 'textfield',
              name: 'bank_name',
              id: 'bank_name',
              submitValue: true,
              fieldLabel: '銀行帳戶',
              hidden: true
          },
        {
            xtype: 'textfield',
            name: 'bank_branch',
            id: 'bank_branch',
            submitValue: true,
            fieldLabel: '分行',
            hidden: true
        },
        {
            xtype: 'textfield',
            name: 'bank_account',
            id: 'bank_account',
            submitValue: true,
            fieldLabel: '帳號',
            hidden: true
        },
         {
             xtype: 'textfield',
             name: 'account_name',
             id: 'account_name',
             submitValue: true,
             fieldLabel: '戶名',
             hidden: true
         },
         {
             xtype: 'textareafield',
             name: 'bank_note',
             id: 'bank_note',
             submitValue: true,
             fieldLabel: '退款資訊'
         }, {
             xtype: 'displayfield',
             name: 'createdate',
             id: 'createdate',
             submitValue: true,
             fieldLabel: '建立時間'
         },
         {
             xtype: 'displayfield',
             name: 'updatedate',
             id: 'updatedate',
             submitValue: true,
             fieldLabel: '歸檔時間'
         },
          {
              xtype: 'displayfield',
              name: 'return_ipfrom',
              id: 'return_ipfrom',
              submitValue: true,
              fieldLabel: '修改來源'
          }
        ],
        buttons: [{
            formBind: true,
            disabled: true,
            text: '修改',
            handler: function () {
                var form = this.up('form').getForm();
                if (form.isValid()) {
                    form.submit({
                        params: {
                            return_id: Ext.htmlEncode(Ext.getCmp('return_id').getValue()),
                            invoice_deal: Ext.htmlEncode(Ext.getCmp('invoice_deal').getValue()),
                            return_deal: Ext.htmlEncode(Ext.getCmp('package').getValue()),
                            return_zip: Ext.htmlEncode(Ext.getCmp('return_zip').getValue()),
                            return_address: Ext.htmlEncode(Ext.getCmp('return_address').getValue()),
                            deliver_code: Ext.htmlEncode(Ext.getCmp('deliver_code').getValue()),
                            return_status: Ext.getCmp('return_statuss').getValue().returnstatus,
                            return_note: Ext.htmlEncode(Ext.getCmp('return_note').getValue()),
                            order_payment: Ext.htmlEncode(Ext.getCmp('order_payment').getValue()),
                            bank_name: Ext.htmlEncode(Ext.getCmp('bank_name').getValue()),
                            bank_branch: Ext.htmlEncode(Ext.getCmp('bank_branch').getValue()),
                            bank_account: Ext.htmlEncode(Ext.getCmp('bank_account').getValue()),
                            account_name: Ext.htmlEncode(Ext.getCmp('account_name').getValue()),
                            bank_note: Ext.htmlEncode(Ext.getCmp('bank_note').getValue())
                        },

                        success: function (form, action) {
                            var result = Ext.decode(action.response.responseText);
                            if (result.success) {
                                Ext.Msg.alert(INFORMATION, SUCCESS);
                                ReturnMasterStore.load();
                                editWin.close();
                            }
                            else {
                                Ext.Msg.alert(INFORMATION, FAILURE);
                            }
                        },
                        failure: function (form, action) {
                            var result = Ext.decode(action.response.responseText);
                            Ext.Msg.alert(INFORMATION, FAILURE);
                        }
                    });

                }

            }
        }]
    });
    var editWin = Ext.create('Ext.window.Window', {
        title: '發票寄送地址修改',
        iconCls: 'icon-user-edit',
        width: 600,
        y: 100,
        layout: 'fit',
        items: [editFrm],
        constrain: true, //束縛
        closeAction: 'destroy',
        modal: true,
        closable: false,
        labelWidth: 60,
        bodyStyle: 'padding:5px 5px 5px 5px',
        tools: [
         {
             type: 'close',
             qtip: '是否關閉',
             handler: function (event, toolEl, panel) {
                 Ext.MessageBox.confirm(CONFIRM, IS_CLOSEFORM, function (btn) {
                     if (btn == "yes") {
                         editWin.destroy();
                     }
                     else {
                         return false;
                     }
                 });
             }
         }],
        listeners: {
            'show': function () {
                editFrm.getForm().loadRecord(row);
                initForm(row);
                var status = row.data.return_status.toString();
                if (status == "0") {
                    Ext.getCmp('radio1').setValue(true);
                    Ext.getCmp('radio2').setValue(false);
                    Ext.getCmp('radio3').setValue(false);
                }
                else if (status == "1") {
                    Ext.getCmp('radio1').setValue(false);
                    Ext.getCmp('radio2').setValue(true);
                    Ext.getCmp('radio3').setValue(false);
                }
                else if (status == "2") {
                    Ext.getCmp('radio1').setValue(false);
                    Ext.getCmp('radio2').setValue(false);
                    Ext.getCmp('radio3').setValue(true);
                }

            }
        }
    });
    editWin.show();

    function initForm(row) {
        if (row.data.return_zip == "0") {
            Ext.getCmp('return_zip').setValue('');
        }
        Ext.Ajax.request({
           url: '/OrderManage/GetPayment',
            method: 'post',
            params: {
                order_id: row.data.order_id
            },
            success: function (form, action) {
                var result = Ext.decode(form.responseText);
                if (result.success) {
                    var order_payment = result.Order_Payment.toString();
                    Ext.getCmp('order_payment').setValue(order_payment);
                    if (order_payment == "2") {
                        Ext.getCmp('bank_name').show();
                        Ext.getCmp('bank_name').setValue(result.bank_name);
                        Ext.getCmp('bank_branch').show();
                        Ext.getCmp('bank_branch').setValue(result.bank_branch);
                        Ext.getCmp('bank_account').show();
                        Ext.getCmp('bank_account').setValue(result.bank_account);
                        Ext.getCmp('account_name').show();
                        Ext.getCmp('account_name').setValue(result.account_name);
                    }
                }
                else {
                   Ext.Msg.alert(INFORMATION, FAILURE);
                }
            },
            failure: function (form, action) {
                var result = Ext.decode(action.response.responseText);
              Ext.Msg.alert(INFORMATION, FAILURE);
            }
        })
        if (row.data.updatedate == '0001-01-01 00:00:00') {
            Ext.getCmp("updatedate").setValue("");
        }
    }
}
