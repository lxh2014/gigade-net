var Vendor_id;
Ext.onReady(function () {
    Vendor_id = document.getElementById('Vendor_id').value;
        createForm();
});

function createForm() {
    var first = Ext.create('Ext.tab.Panel', {
        id: 'ContentPanel',
        region: 'center',
        margins: '0 5 0 0',
        activeTab: 0,
        tabPosition:'bottom',
        items: [
             {
                 title: '基本信息',
                 margin: '8 10 15 8',
                 anchor: "95%", msgTarget: "side",
                 border: false,
                 autoScroll: true,
                 labelWidth: 200,
                 items: [
                        {
                            xtype: 'container',
                            layout: 'column',
                            id: 'product_brand_name',
                            margin: '5 8 5 15',
                            //hidden: true,
                            defaults: {
                                width: 300,
                                labelWidth: 80
                            },
                            items: [
                                {
                                    fieldLabel: '供應商編號',
                                    xtype: 'displayfield',
                                    id: 'vendor_id',
                                    name: 'vendor_id'
                                },
                                {
                                    fieldLabel: '供應商編碼',
                                    xtype: 'displayfield',
                                    id: 'vendor_code',
                                    name: 'vendor_code'
                                }

                            ], style: { borderBottom: '1px solid #ced9e7' }
                        },
                        {
                            xtype: 'container',
                            layout: 'column',
                            margin: '5 8 5 15',
                            defaults: {
                                width: 300,
                                labelWidth: 80

                            },
                            items: [
                                {
                                    fieldLabel: '供應商名稱',
                                    xtype: 'displayfield',
                                    id: 'vendor_name_full',
                                    name: 'vendor_name_full'
                                },
                                {
                                    fieldLabel: '供應商簡稱',
                                    xtype: 'displayfield',
                                    id: 'vendor_name_simple',
                                    name: 'vendor_name_simple' 
                                }
                            ], style: { borderBottom: '1px solid #ced9e7' }
                        },
                        {
                            xtype: 'container',
                            layout: 'column',
                            margin: '5 8 5 15',
                            defaults: {
                                width: 300,
                                labelWidth: 80
                            },
                            items: [
                                {
                                    fieldLabel: '統一編號',
                                    xtype: 'displayfield',
                                    id: 'vendor_invoice',
                                    name: 'vendor_invoice'
                                },
                                {
                                    fieldLabel: '公司Email',
                                    xtype: 'displayfield',
                                    id: 'vendor_email',
                                    name: 'vendor_email'
                                }
                            ], style: { borderBottom: '1px solid #ced9e7' }
                        },
                        {
                            xtype: 'container',
                            layout: 'column',
                            margin: '5 8 5 15',
                            defaults: {
                                width: 300,
                                labelWidth: 80,

                            },
                            items: [
                                {
                                   
                                    id: 'vendor_status',
                                    name: 'vendor_status',
                                    fieldLabel: '狀態',
                                    xtype: 'displayfield'
                                }
                               
                            ], style: { borderBottom: '1px solid #ced9e7' }
                        },
                        {
                            xtype: 'container',
                            layout: 'column',
                            margin: '5 8 5 15',
                            defaults: {
                                width: 300,
                                labelWidth: 80
                            },
                            items: [
                                {
                                    fieldLabel: '公司電話',
                                    xtype: 'displayfield',
                                    id: 'company_phone',
                                    name: 'company_phone'
                                },
                                {
                                    fieldLabel: '公司傳真',
                                    xtype: 'displayfield',
                                    id: 'company_fax',
                                    name: 'company_fax'
                                }
                            ], style: { borderBottom: '1px solid #ced9e7' }
                        },
                        {
                            xtype: 'container',
                            layout: 'column',
                            margin: '5 8 5 15',
                            defaults: {
                                width: 300,
                                labelWidth: 80
                            },
                            items: [
                            {
                                fieldLabel: '公司負責人',
                                xtype: 'displayfield',
                                id: 'company_person',
                                name: 'company_person'

                            },
                            {
                                fieldLabel: '供應商類型',
                                xtype: 'displayfield',
                                id: 'vendor_type',
                                name: 'vendor_type'

                            }
                            ], style: { borderBottom: '1px solid #ced9e7' }
                        },
                        {
                            xtype: 'container',
                            layout: 'column',
                            margin: '5 8 5 15',
                            defaults: {
                                hideLabel: true,
                            },
                            items: [
                                {
                                    xtype: 'displayfield',
                                    value: '公司地址:',
                                    //id: 'company_address',
                                    width: 60
                                },
                                 {
                                     xtype: 'displayfield',
                                     id: 'cob_ccity_a',
                                     name: 'cob_ccity_a',
                                     width: 140
                                 },
                                {
                                    id: 'company_address',
                                    xtype: 'displayfield',
                                    name: 'company_address',
                                    width: 500
                                }
                            ], style: { borderBottom: '1px solid #ced9e7' }
                        },
                        {
                            xtype: 'container',
                            layout: 'column',
                            margin: '5 8 5 15',
                            defaults: {
                                hideLabel: true
                            },
                            items: [
                                {
                                    xtype: 'displayfield',
                                    value: '發票地址:',
                                    //id: 'company_address',
                                    width: 60
                                },
                                {
                                     xtype: 'displayfield',
                                     id: 'cob_ccity_ai',
                                     name: 'cob_ccity_ai',
                                     width: 140
                                },
                                {
                                    id: 'invoice_address',
                                    xtype: 'displayfield',
                                    width: 500,
                                    name: 'invoice_address'
                                }
                            ], style: { borderBottom: '1px solid #ced9e7' }
                        },
                        {
                            xtype: 'container',
                            layout: 'column',
                            margin: '5 8 5 15',
                            defaults: {
                                width: 300,
                                labelWidth: 80,

                            },
                            items: [
                                {
                                    xtype: 'displayfield',
                                    name: 'cost_percent',
                                    id: 'cost_percent',
                                    width: 80,
                                    labelWidth: 50,
                                    // allowDecimals: false,
                                    fieldLabel: '成本',
                                    //allowBlank: false,
                                    //minValue: 0,
                                    //maxValue: 100
                                },
                                {
                                    xtype: 'displayfield',
                                    value:'百分比'// "<font style='color:red'>" + '%'+ "</font>"
                                }
                            ], style: { borderBottom: '1px solid #ced9e7' }
                        },
                        {

                                xtype: 'container',
                                layout: 'column',
                                margin: '5 8 5 15',
                                defaults: {
                                    width: 300,
                                    labelWidth: 80,

                                },
                                items: [
                                    {
                                        fieldLabel: '採購天數',
                                        xtype: 'displayfield',

                                        id: 'procurement_days',
                                        name: 'procurement_days'
                                    },
                                    {
                                        fieldLabel: '自出出貨天數',
                                        xtype: 'displayfield',
                                        id: 'self_send_days',

                                        name: 'self_send_days'
                                    }
                                ], style: { borderBottom: '1px solid #ced9e7' }
                            },
                        {
                            xtype: 'container',
                            layout: 'column',
                            margin: '5 8 5 15',
                            defaults: {
                                width: 300,
                                labelWidth: 80,

                            },
                            items: [
                                {
                                    fieldLabel: '寄倉出貨天數',
                                    xtype: 'displayfield',
                                    id: 'stuff_ware_days',
                                    name: 'stuff_ware_days'
                                },
                                {
                                    fieldLabel: '調度出貨天數',
                                    xtype: 'displayfield',
                                    id: 'dispatch_days',
                                    name: 'dispatch_days'

                                }
                            ], style: { borderBottom: '1px solid #ced9e7' }
                        },
                        {
                            xtype: 'container',
                            layout: 'column',
                            margin: '5 8 5 15',
                            defaults: {
                                width: 300,
                                labelWidth: 80,

                            },
                            items: [
                                    {
                                        xtype: 'fieldcontainer',
                                        combineErrors: true,
                                        layout: 'hbox',
                                        items: [
                                            {
                                                xtype: 'displayfield',
                                                name: 'creditcard_1_percent',
                                                id: 'creditcard_1_percent',
                                                //labelWidth: 120,
                                                //allowDecimals: false,
                                                fieldLabel: '信用卡一期手續費',
                                                //minValue: 0,
                                                //maxValue: 100
                                            },
                                            {
                                                xtype: 'displayfield',
                                                width: 40,
                                                margin: '0 0 0 15',
                                                value: '百分比' //"<font style='color:red'>" + '%' + "</font>"
                                            }
                                        ]
                                    },
                                    {
                                        xtype: 'container',
                                        layout: 'column',
                                        items: [
                                             {
                                                 xtype: 'displayfield',
                                                 name: 'creditcard_3_percent',
                                                 id: 'creditcard_3_percent',
                                                 // labelWidth: 120,
                                                 allowDecimals: false,
                                                 fieldLabel: '信用卡三期手續費',
                                                 // minValue: 0,
                                                 // maxValue: 10
                                                 //,
                                                 //regex: /^(([0-9]+\.[0-9]*[1-9][0-9]*)|([0-9]*[1-9][0-9]*\.[0-9]+)|([0-9]*[1-9][0-9]*))$/
                                             },
                                             {
                                                 xtype: 'displayfield',
                                                 width: 40,
                                                 margin: '0 0 0 15',
                                                 value: '百分比'// "<font style='color:red'>" + '%' + "</font>"
                                             }
                                        ]
                                    }
                            ], style: { borderBottom: '1px solid #ced9e7' }
                        },
                        {

                            xtype: 'container',
                            layout: 'column',
                            margin: '5 8 5 15',
                            defaults: {
                                width: 300,
                                labelWidth: 80,

                            },
                            items: [
                                 {
                                     xtype: "displayfield",
                                     id: 'agreement_createdate',
                                     name: 'agreement_createdate',
                                     // format: 'Y-m-d',
                                     labelWidth: 80,
                                     fieldLabel: '合約簽訂日期',
                                     //allowBlank: false,
                                     //submitValue: true,
                                     //value: Tomorrow()
                                 },
                                 {
                                     xtype: 'fieldcontainer',
                                     combineErrors: true,
                                     layout: 'hbox',
                                     labelWidth: 60,
                                     fieldLabel: '合約期間',
                                     items: [
                                         {
                                             xtype: "displayfield",
                                             id: 'agreement_start',
                                             name: 'agreement_start',
                                             format: 'Y-m-d',
                                             width: 80

                                         },
                                         {
                                             xtype: 'displayfield',
                                             value: '~',
                                             margin: '0 8 0 0'
                                         },
                                         {
                                             xtype: "displayfield",
                                             format: 'Y-m-d',
                                             id: 'agreement_end',
                                             width: 100,

                                             name: 'agreement_end',

                                         }
                                     ]
                                 },

                            ], style: { borderBottom: '1px solid #ced9e7' }
                        },
                        {
                            xtype: 'container',
                            layout: 'column',
                            margin: '5 8 5 15',
                            defaults: {
                                width: 300,
                                labelWidth: 120
                            },
                            items: [
                                  {
                                      xtype: 'displayfield',
                                      width: 120,
                                      value: '供應商結賬方式:'
                                  },
                                  {
                                      xtype: 'displayfield',
                                      id: 'checkout_type',
                                      width: 120
                                  }
                               
                            ], style: { borderBottom: '1px solid #ced9e7' }
                        },
                    
                 ]
             },
             {
                 title: '其他信息',
                 margin: '8 10 15 8',
                 anchor: "95%", msgTarget: "side",
                 border: false,
                 autoScroll: true,
                 labelWidth: 200,
                 items: [
                       {
                           xtype: 'container',
                           layout: 'column',
                           margin: '5 8 5 15',
                           defaults: {
                               width: 300,
                               labelWidth: 80
                           },
                           items: [
                               {
                                   fieldLabel: '銀行代碼',
                                   xtype: 'displayfield',
                                   id: 'bank_code',
                                   name: 'bank_code'
                               },
                               {
                                   fieldLabel: '銀行名稱',
                                   xtype: 'displayfield',
                                   id: 'bank_name',
                                   name: 'bank_name'
                               }
                           ], style: { borderBottom: '1px solid #ced9e7' }
                       },
                       {
                           xtype: 'container',
                           layout: 'column',
                           margin: '5 8 5 15',
                           defaults: {
                               width: 300,
                               labelWidth: 80
                           },
                           items: [
                               {
                                   fieldLabel: '銀行帳號',
                                   xtype: 'displayfield',
                                   id: 'bank_number',
                                   name: 'bank_number'
                               },
                               {
                                   fieldLabel: '銀行戶名',
                                   xtype: 'displayfield',
                                   id: 'bank_account',
                                   name: 'bank_account',
                               }
                           ], style: { borderBottom: '1px solid #ced9e7' }
                       },
                       {

                           xtype: 'container',
                           margin: '5 8 5 15',
                           layout: 'column',
                           defaults: {
                               labelWidth: 100,
                           },
                           items: [
                                {
                                    xtype: 'displayfield',
                                    value: '常溫商品:',
                                    margin: '0 10 0 0',
                                },
                               {
                                   xtype: 'displayfield',
                                   value: '運費'
                               },
                               {
                                   xtype: 'displayfield',
                                   id: 'freight_normal_money',
                                   name: 'freight_normal_money',
                                   margin: '0 5 0 5',
                               },
                               {
                                   xtype: 'displayfield',
                                   value: '元，運費'
                               },
                               {
                                   xtype: 'displayfield',
                                   id: 'freight_normal_limit',
                                   name: 'freight_normal_limit',
                                   margin: '0 5 0 5',
                               },
                               {
                                   xtype: 'displayfield',
                                   value: '元(含)以上免運費，逆物流運費'
                               },
                               {
                                   xtype: 'displayfield',
                                   id: 'freight_return_normal_money',
                                   name: 'freight_return_normal_money',
                                   margin: '0 5 0 5',
                               },
                               {
                                   xtype: 'displayfield',
                                   value: '元'
                               }
                           ], style: { borderBottom: '1px solid #ced9e7' }
                       },
                       {
                           xtype: 'container',
                           layout: 'column',
                           margin: '5 8 5 15',
                           defaults: {
                               labelWidth: 100
                           },
                           items: [
                               {
                                   xtype: 'displayfield',
                                   margin: '0 10 0 0',
                                   value: '低溫商品:',

                               },
                               {
                                   xtype: 'displayfield',
                                   value: '運費'
                               },
                               {
                                   xtype: 'displayfield',
                                   id: 'freight_low_money',
                                   name: 'freight_low_money',
                                   margin: '0 5 0 5',
                               },
                               {
                                   xtype: 'displayfield',
                                   value: '元，運費'
                               },
                               {
                                   xtype: 'displayfield',
                                   id: 'freight_low_limit',
                                   name: 'freight_low_limit',
                                   margin: '0 5 0 5',
                               },
                               {
                                   xtype: 'displayfield',
                                   value: '元(含)以上免運費，逆物流運費'
                               },
                               {
                                   xtype: 'displayfield',
                                   id: 'freight_return_low_money',
                                   name: 'freight_return_low_money', 
                                   margin: '0 5 0 5',
                               },
                               {
                                   xtype: 'displayfield',
                                   value: '元'
                               }
                           ], style: { borderBottom: '1px solid #ced9e7' }
                       },
                       {
                           xtype: 'container',
                           layout: 'column',
                           margin: '5 8 5 15',
                           defaults: {
                               width: 300,
                               labelWidth: 80
                           },
                           items: [
                               {
                                   xtype: 'displayfield',

                                   fieldLabel: '管理人員',
                                   // hidden: true,//先隱藏掉,以免以后會再用
                                   id: 'manage_name',
                                   name: 'manage_name',

                               },
                               {
                                   xtype: 'displayfield',
                                   fieldLabel: 'ERP 廠商編號',
                                   id: 'erp_id',
                                   name: 'erp_id'
                               },

                           ], style: { borderBottom: '1px solid #ced9e7' }
                       },
                       {
                           xtype: 'container',
                           layout: 'column',
                           margin: '5 8 5 15',
                           defaults: {
                               width: 300,
                               labelWidth: 80
                           },
                           items: [
                               {
                                   xtype: 'displayfield',
                                   fieldLabel: '業績獎金門檻',
                                   id: 'gigade_bunus_threshold',
                                   name: 'gigade_bunus_threshold'
                               },
                                {
                                    xtype: 'fieldcontainer',
                                    combineErrors: true,
                                    layout: 'hbox',
                                    items: [
                                        {
                                            xtype: 'displayfield',
                                            fieldLabel: '獎金百分比',
                                            id: 'gigade_bunus_percent',
                                            name: 'gigade_bunus_percent',
                                            labelWidth: 80
                                        },
                                        {
                                            xtype: 'displayfield',
                                            margin: '0 0 0 5',
                                            value: '百分比'//"<font style='color:red'>" + '百分比' + "</font>"
                                        }
                                    ]
                                }
                              
                           ], style: { borderBottom: '1px solid #ced9e7' }
                       },
                       {
                           xtype: 'container',
                           layout: 'column',
                           margin: '5 8 5 15',
                           defaults: {
                               width: 602
                           },
                           items: [
                               gdcontact,
                               {
                                   xtype: 'textareafield',
                                   fieldLabel: '備註',
                                   id: 'vendor_note',
                                   name: 'vendor_note',
                                   margin: '5 0 5 0',
                                   readOnly: true,
                                   labelWidth: 50,
                                   width: 480
                               }
                           ]
                       }
                 ]
             },
            
        ],
        listeners: {
            'afterrender': function () {
                Ext.Ajax.request({
                    url: '/Vendor/GetVendorList',
                    method: 'post',
                    reader: {
                        type: 'json',
                        root: 'data'
                    },
                    params: {
                        Vendorid: Vendor_id
                    },
                    success: function (form, action) {
                        var result = Ext.decode(form.responseText);
                        if (result.success) {
                            /****************************************頁面一******************************************************/
                            Ext.getCmp('vendor_id').setValue(result.data[0].vendor_id);
                            Ext.getCmp('vendor_code').setValue(result.data[0].vendor_code);
                            Ext.getCmp('vendor_name_full').setValue(result.data[0].vendor_name_full);
                            Ext.getCmp('vendor_name_simple').setValue(result.data[0].vendor_name_simple);
                            Ext.getCmp('vendor_invoice').setValue(result.data[0].vendor_invoice);
                            Ext.getCmp('vendor_email').setValue(result.data[0].vendor_email);
                            switch (result.data[0].vendor_status) {
                                case 1:
                                    Ext.getCmp("vendor_status").setValue("啟用");
                                    break;
                                case 2:
                                    Ext.getCmp("vendor_status").setValue("停用");
                                    break;
                                case 3:
                                    Ext.getCmp("vendor_status").setValue("失格");
                                    break;
                            }
                            //Ext.getCmp('vendor_password').setValue(result.data[0].vendor_password.substr(0,1) + "***");
                            Ext.getCmp('company_phone').setValue(result.data[0].company_phone);
                            Ext.getCmp('company_fax').setValue(result.data[0].company_fax);//vendor_type
                            Ext.getCmp('company_person').setValue(result.data[0].company_person);
                            Ext.Ajax.request({
                                url: "/Vendor/GetVendorType",
                                params: {
                                    VendorType: result.data[0].vendor_type
                                },
                                success: function (response) {

                                    var result = Ext.decode(response.responseText);
                                   
                                    Ext.getCmp("vendor_type").setValue(result.msg);
                                   
                                },
                                failure: function (form, action) {
                                    Ext.Msg.alert(INFORMATION, "系統出現錯誤!");
                                }
                            });
                            //alert(result.data[0].vendor_type)
                            //switch (result.data[0].vendor_type) {
                            //    case '1':
                            //        Ext.getCmp("vendor_type").setValue("食品供應商");
                            //        break;
                            //    case '2':
                            //        Ext.getCmp("vendor_type").setValue("用品供應商");
                            //        break;
                            //    case '3':
                            //        Ext.getCmp("vendor_type").setValue("休閒供應商");
                            //        break;
                            //}
                           // Ext.getCmp('vendor_type').setValue(VendorTypeStore.getAt(VendorTypeStore.find("vendor_type", '' + result.data[0].vendor_type + '')).data.vendor_type_name);
                            Ext.Ajax.request({
                                url: "/Vendor/GetZip",
                                params: {
                                    big_code:result.data[0].c_bigcode,
                                    c_midcode:result.data[0].c_midcode,
                                    c_zipcode:result.data[0].c_zipcode
                                },
                                success: function (response) {
                                  
                                    var result = Ext.decode(response.responseText);
                                    if (result.msg == "100") {
                                        Ext.getCmp("cob_ccity_a").setValue('');
                                       
                                    }
                                    else {
                                        Ext.getCmp("cob_ccity_a").setValue(result.msg);
                                    }
                                },
                                failure: function (form, action) {
                                    Ext.Msg.alert(INFORMATION, "系統出現錯誤!");
                                }
                            });
                            Ext.getCmp('company_address').setValue(result.data[0].company_address.trim());
                            Ext.Ajax.request({
                                url: "/Vendor/GetZip",
                                params: {
                                    big_code: result.data[0].i_bigcode,
                                    c_midcode: result.data[0].i_midcode,
                                    c_zipcode: result.data[0].i_zipcode
                                },
                                success: function (response) {

                                    var result = Ext.decode(response.responseText);
                                    if (result.msg == "100") {
                                        Ext.getCmp("cob_ccity_ai").setValue('');

                                    }
                                    else {
                                        Ext.getCmp("cob_ccity_ai").setValue(result.msg);
                                    }
                                },
                                failure: function (form, action) {
                                    Ext.Msg.alert(INFORMATION, "系統出現錯誤!");
                                }
                            });
                            Ext.getCmp('invoice_address').setValue(result.data[0].invoice_address.trim());
                            Ext.getCmp('cost_percent').setValue(result.data[0].cost_percent);
                            Ext.getCmp('procurement_days').setValue(result.data[0].procurement_days);
                            Ext.getCmp('self_send_days').setValue(result.data[0].self_send_days);
                            Ext.getCmp('stuff_ware_days').setValue(result.data[0].stuff_ware_days);
                            Ext.getCmp('dispatch_days').setValue(result.data[0].dispatch_days);
                            Ext.getCmp('creditcard_1_percent').setValue(result.data[0].creditcard_1_percent);
                            Ext.getCmp('creditcard_3_percent').setValue(result.data[0].creditcard_3_percent);//agreement_createdate
                            if (result.data[0].agr_date !== "undefined") {
                                Ext.getCmp('agreement_createdate').setValue(result.data[0].agr_date.substring(0, 10));
                            }
                            if (result.data[0].agr_start !== "undefined") {
                                Ext.getCmp('agreement_start').setValue(result.data[0].agr_start.substring(0, 10));
                            }
                            if (result.data[0].agr_end !== "undefined") {
                                Ext.getCmp('agreement_end').setValue(result.data[0].agr_end.substring(0, 10));
                            }
                            switch (result.data[0].checkout_type) {
                                case 1:
                                    Ext.getCmp("checkout_type").setValue("月結");
                                    break;
                                case 2:
                                    Ext.getCmp("checkout_type").setValue("半月結");
                                    break;
                                case 3:
                                    Ext.getCmp("checkout_type").setValue("<其他> " + result.data[0].checkout_other);
                                    break;

                            }
                            /****************************************頁面二******************************************************/
                            Ext.getCmp('bank_code').setValue(result.data[0].bank_code);
                            Ext.getCmp('bank_name').setValue(result.data[0].bank_name);
                            Ext.getCmp('bank_number').setValue(result.data[0].bank_number);
                            Ext.getCmp('bank_account').setValue(result.data[0].bank_account);//freight_normal_limit
                            Ext.getCmp('freight_normal_money').setValue("<font style='color:red'>" +result.data[0].freight_normal_money+"</font>");
                            Ext.getCmp('freight_normal_limit').setValue("<font style='color:red'>" + result.data[0].freight_normal_limit + "</font>");
                            Ext.getCmp('freight_return_normal_money').setValue("<font style='color:red'>" + result.data[0].freight_return_normal_money + "</font>");//gigade_bunus_percent
                            Ext.getCmp('freight_low_money').setValue("<font style='color:red'>" + result.data[0].freight_low_money + "</font>");
                            Ext.getCmp('freight_low_limit').setValue("<font style='color:red'>" + result.data[0].freight_low_limit + "</font>");
                            Ext.getCmp('freight_return_low_money').setValue("<font style='color:red'>" + result.data[0].freight_return_low_money + "</font>");
                            Ext.getCmp('manage_name').setValue(result.data[0].manage_name);
                            Ext.getCmp('erp_id').setValue(result.data[0].erp_id);
                            Ext.getCmp('gigade_bunus_threshold').setValue(result.data[0].gigade_bunus_threshold);
                            Ext.getCmp('gigade_bunus_percent').setValue(result.data[0].gigade_bunus_percent);//vendor_note
                            /****************************************頁面三******************************************************/
                            ContactStore.load({ params: { vendor_id: Vendor_id } });
                            Ext.getCmp('vendor_note').setValue(result.data[0].vendor_note);
                          
                        }
                    }
                })
            }
        }
          
    });
    Ext.create('Ext.Viewport', {
        layout: 'border',
        items: [first]
    })
}
