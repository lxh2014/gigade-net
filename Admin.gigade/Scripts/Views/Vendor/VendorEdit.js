var global_row = null;
var old_name = "";
function editFunction(rowID) {
    var VendorID = null;
    var currentPanel = 0;
    var comArea = false;
    var comArea_i = false;
    var comCity = false;
    var comCity_i = false;
    var delconnect = '';


    //區域Model
    Ext.define('gigade.Area', {
        extend: 'Ext.data.Model',
        fields: [
            { name: "big", type: "string" },
            { name: "bigcode", type: "string" }
        ]
    });
    //地址Model
    Ext.define('gigade.City', {
        extend: 'Ext.data.Model',
        fields: [
            { name: "middle", type: "string" },
            { name: "middlecode", type: "string" }
        ]
    });

    //郵編Model
    Ext.define('gigade.Zip', {
        extend: 'Ext.data.Model',
        fields: [
            { name: "zipcode", type: "string" },
            { name: "small", type: "string" }
        ]
    });
    //公司地址郵編Store
    var AreaStore = Ext.create('Ext.data.Store', {
        autoDestroy: true,
        model: 'gigade.Area',
        remoteSort: false,
        autoLoad: true,
        proxy: {
            type: 'ajax',
            url: "/Vendor/QueryArea",
            reader: {
                type: 'json',
                root: 'items'
            }
        }
    });
    //供應商類型model/VendorTypeStore
    Ext.define('gigade.VendorType', {
        extend: 'Ext.data.Model',
        fields: [
            { name: "vendor_type", type: "string" },
            { name: "vendor_type_name", type: "string" }
        ]
    });
    //供應商類型store
    var VendorTypeStore = Ext.create('Ext.data.Store', {
        model: 'gigade.VendorType',
        autoLoad: true,
        proxy: {
            type: 'ajax',
            url: "/Vendor/GetVendorTypeStore",
            reader: {
                type: 'json',
                root: 'data'
            }
        }
    });

    //公司地址郵編Store
    var CityStore = Ext.create('Ext.data.Store', {
        autoDestroy: true,
        model: 'gigade.City',
        remoteSort: false,
        autoLoad: false,
        proxy: {
            type: 'ajax',
            url: "/Vendor/QueryCity",
            reader: {
                type: 'json',
                root: 'items'
            }
        }
    });

    //公司地址郵編Store
    var ZipStore = Ext.create('Ext.data.Store', {
        autoDestroy: true,
        remoteSort: false,
        autoLoad: false,
        model: 'gigade.Zip',
        proxy: {
            type: 'ajax',
            url: "/Vendor/QueryZip",
            actionMethods: 'post',
            reader: {
                type: 'json',
                root: 'items'
            }
        }
    });
    //公司地址郵編Store
    var InvoiceAreaStore = Ext.create('Ext.data.Store', {
        autoDestroy: true,
        model: 'gigade.Area',
        remoteSort: false,
        autoLoad: true,
        proxy: {
            type: 'ajax',
            url: "/Vendor/QueryArea",
            reader: {
                type: 'json',
                root: 'items'
            }
        }
    });
    //發票地址Store
    var InvoiceCityStore = Ext.create('Ext.data.Store', {
        autoDestroy: true,
        model: 'gigade.City',
        remoteSort: false,
        autoLoad: false,
        proxy: {
            type: 'ajax',
            url: "/Vendor/QueryCity",
            reader: {
                type: 'json',
                root: 'items'
            }
        }
    });

    //發票地址郵編Store
    var InvoiceZipStore = Ext.create('Ext.data.Store', {
        autoDestroy: true,
        remoteSort: false,
        autoLoad: false,
        model: 'gigade.Zip',
        proxy: {
            type: 'ajax',
            url: "/Vendor/QueryZip",
            actionMethods: 'post',
            reader: {
                type: 'json',
                root: 'items'
            }
        }
    });


    var InvoiceStore = Ext.create('Ext.data.Store', {
        model: 'gigade.Zip',
        autoLoad: true,
        proxy: {
            type: 'ajax',
            url: "/Vendor/GetZipAddress",
            actionMethods: 'post',
            reader: {
                type: 'json',
                root: 'data'
            }
        }
    });

    //管理人員
    Ext.define("gigade.pm", {
        extend: 'Ext.data.Model',
        fields: [
            { name: "userId", type: "string" },
            { name: "userName", type: "string" }
        ]
    });
    //pmstore
    var pmStore = Ext.create('Ext.data.Store', {
        model: 'gigade.pm',
        autoLoad: true,
        proxy: {
            type: 'ajax',
            url: "/Vendor/QueryPm",
            actionMethods: 'post',
            reader: {
                type: 'json',
                root: 'item'
            }
        }
    });

    //商品類型Model
    Ext.define('gigade.erp_cate', {
        extend: 'Ext.data.Model',
        fields: [
            { name: "ParameterCode", type: "string" },
            { name: "parameterName", type: "string" }
        ]
    });

    //商品類型Store
    var ProductCateStore = Ext.create('Ext.data.Store', {
        autoDestroy: true,
        model: 'gigade.erp_cate',
        remoteSort: false,
        autoLoad: true,
        proxy: {
            type: 'ajax',
            url: "/Vendor/QueryProductCate",
            reader: {
                type: 'json',
                root: 'items'
            }
        }
    });
    //採購類型Store
    var BuyCateStore = Ext.create('Ext.data.Store', {
        autoDestroy: true,
        remoteSort: false,
        autoLoad: false,
        model: 'gigade.erp_cate',
        proxy: {
            type: 'ajax',
            url: "/Vendor/QueryBuyCate",
            actionMethods: 'post',
            reader: {
                type: 'json',
                root: 'items'
            }
        }
    });
    BuyCateStore.on('beforeload', function () {
        Ext.apply(BuyCateStore.proxy.extraParams, {
            topValue: Ext.getCmp("prod_cate").getValue()
        });
    });

    //應稅免稅Model
    Ext.define("gigade.taxtype", {
        extend: 'Ext.data.Model',
        fields: [
            { name: "id", type: "int" },
            { name: "value", type: "string" }
        ]
    });
    //應稅免稅store
    var taxTypeStore = Ext.create('Ext.data.Store', {
        model: 'gigade.taxtype',
        autoLoad: true,
        data: [
            //{ id: '0', value: '請選擇應稅免稅' },
            { id: '1', value: '應稅' },
            { id: '2', value: '免稅' }
        ]
    });

    //聯繫人
    Ext.define("gigade.connecttype", {
        extend: 'Ext.data.Model',
        fields: [
            { name: "value", type: "string" }
        ]
    });
    var connecttypeStore = Ext.create('Ext.data.Store', {
        model: 'gigade.connecttype',
        autoLoad: true,
        data: [
            { value: '請選擇' },
            { value: '負責人' },
            { value: '業務窗口' },
            { value: '圖/文窗口' },
            { value: '出貨負責窗口' },
            { value: '帳務連絡窗口' },
            { value: '客服窗口' }
        ]
    });

    //聯絡人gridModel
    Ext.define('Contact', {
        extend: 'Ext.data.Model',
        fields: [
            { name: "rid", type: "string" },
            { name: "contact_type", type: "string" },
            { name: "contact_name", type: "string" },
            { name: "contact_phone1", type: "string" },
            { name: "contact_phone2", type: "string" },
            { name: "contact_mobile", type: "string" },
            { name: "contact_email", type: "string" }
        ]
    });

    var ContactStore = Ext.create('Ext.data.Store', {
        autoDestroy: true,
        model: 'Contact',
        remoteSort: false,
        autoLoad: false,
        proxy: {
            type: 'ajax',
            url: "/Vendor/QueryContact",
            reader: {
                type: 'json',
                root: 'items'
            }
        }
    });

    var cellEditing = Ext.create('Ext.grid.plugin.CellEditing', {
        clicksToEdit: 1,
        listeners: {
            beforeedit: function (e, eOpts) {
                if (e.record.data.contact_type === "出貨聯絡窗口") {
                    if (e.rowIdx === 0 && e.colIdx === 1) {
                        return false;
                    }
                }
            }
        }
    });
    var gdcontact = Ext.create('Ext.grid.Panel', {
        id: 'gdcontact',
        store: ContactStore,
        plugins: [cellEditing],
        //width: document.documentElement.clientWidth,
        anchor: '98%',
        height: 220,
        autoScroll: true,
        frame: true,
        columns: [
            {
                header: '功能', xtype: 'actioncolumn', width: 40, align: 'center',
                items: [
                    {
                        icon: '../../../Content/img/icons/cross.gif',
                        tooltip: '刪除',
                        handler: function (grid, rowIndex, colIndex) {
                            Ext.Msg.confirm("注意", "確認刪除？", function (btn) {
                                if (btn === 'yes') {
                                    if (ContactStore.getCount() > 1 && rowIndex != 0) {
                                        delconnect += rowIndex + ",";
                                        ContactStore.removeAt(rowIndex);
                                    } else {
                                        Ext.Msg.alert("注意", "第一行不能刪除");
                                    }
                                }
                            });
                        }
                    }
                ]
            },
            {
                header: '窗口屬性',
                dataIndex: 'contact_type',
                width: 90,
                align: 'left',
                editor: {
                    id: 'txt_contacttype',
                    xtype: 'combobox',
                    typeAhead: true,
                    store: connecttypeStore,
                    displayField: 'value',
                    valueField: 'value',
                    typeAhead: true,
                    value: 0,
                    forceSelection: false,
                    editable: false,
                    queryMode: 'local'
                }
            },
            {
                header: '姓名', dataIndex: 'contact_name', width: 65, align: 'center',
                editor: {
                    // id: 'txt_contactname',
                    listeners: {
                        blur: function (txt, e) {
                            if (txt.rawValue != old_name) {
                                var bre = ContactStore.getAt(ContactStore.findBy(function (record, id) {
                                    return record.data.contact_name == txt.getValue();
                                }));
                                if (bre != undefined) {
                                    ContactStore.getAt(global_row).data.contact_phone1 = bre.data.contact_phone1;
                                    ContactStore.getAt(global_row).data.contact_phone2 = bre.data.contact_phone2;
                                    ContactStore.getAt(global_row).data.contact_mobile = bre.data.contact_mobile;
                                    ContactStore.getAt(global_row).data.contact_email = bre.data.contact_email;
                                }
                                global_row = null;
                                old_name = "";
                            }
                        },
                        specialkey: function (txt, e) {
                            if (txt.rawValue != old_name) {
                                if (e.getKey() == e.ENTER) {
                                    var kre = ContactStore.getAt(ContactStore.findBy(function (record, id) {
                                        return record.data.contact_name == txt.getValue();
                                    }));
                                    if (kre != undefined) {
                                        ContactStore.getAt(global_row).data.contact_phone1 = kre.data.contact_phone1;
                                        ContactStore.getAt(global_row).data.contact_phone2 = kre.data.contact_phone2;
                                        ContactStore.getAt(global_row).data.contact_mobile = kre.data.contact_mobile;
                                        ContactStore.getAt(global_row).data.contact_email = kre.data.contact_email;
                                    }
                                    global_row = null;
                                    old_name = "";
                                }
                            }
                        }
                    }
                }
            },
            {
                header: '電話1', dataIndex: 'contact_phone1', width: 70, align: 'center', editor: { id: 'txt_contactphone' },
                editor: {
                    xtype: "textfield",
                    minLength: 5,
                    maxLength: 20
                    //,
                    //regex: /^([0-9]\d*)$/
                }
            },
            {
                header: '電話2', dataIndex: 'contact_phone2', width: 70, align: 'center', editor: { id: 'txt_contactphone1' },
                editor: {
                    xtype: "textfield",
                    minLength: 5,
                    maxLength: 20
                    //,
                    //regex: /^([0-9]\d*)$/
                }
            },
            {
                header: '手機', dataIndex: 'contact_mobile', width: 70, align: 'center', editor: { id: 'txt_contactmobile' },
                editor: {
                    xtype: "textfield",
                    minLength: 10,
                    maxLength: 10,
                    regex: /^\s*$|([0-9]\d*)$/
                }
            },
            {
                header: 'E-mail', dataIndex: 'contact_email', width: 80, align: 'center', editor: { id: 'txt_contactemail' },
                editor: {
                    xtype: "textfield",
                    sumbitValue: true,
                    vtype: 'email',
                    regex: /^\s*$|([a-z0-9_\.-]+)@([\da-z\.-]+)\.([a-z\.]{2,6})$/,
                    invalidText: 'E-Mail格式必須小寫'
                }
            }
        ],
        tbar: [{ xtype: 'button', text: '新增聯繫人', iconcls: 'icon-user-add', handler: addTr }],
        listeners: {
            beforerender: function () {
                if (!rowID) {
                    ContactStore.load({
                        callback: function (records, operation, success) {
                            //alert(records.length)
                            if (records.length <= 0) {
                                addTr();
                            }
                        }
                    });
                }
            },
            beforeedit: function (grid) {
                global_row = grid.rowIdx;
                old_name = ContactStore.getAt(global_row).data.contact_name;
            }
        }
    });

    var navigate = function (panel, direction) {
        var layout = panel.getLayout();
        // var move = Ext.getCmp('move-next').text;
        if ('next' === direction) {
            if (currentPanel === 0) {
                var formf = firstForm.getForm();
                //驗證出貨天數的合法性
                var zc = Ext.htmlEncode(Ext.getCmp('self_send_days').getValue());
                var jc = Ext.htmlEncode(Ext.getCmp('stuff_ware_days').getValue());
                var dd = Ext.htmlEncode(Ext.getCmp('dispatch_days').getValue());

                var isSave = true;
                if (dd + jc + zc <= 0) {
                    isSave = false;
                    Ext.Msg.alert(INFORMATION, '請填寫出貨天數！');
                    return false;
                }
                if (formf.isValid()) {
                    if (!rowID) {//新增
                        if (Ext.getCmp('cost_percent').getValue() < 50) {
                            Ext.Msg.confirm("注意", "是否確定成本百分比小於50？", function (btn) {
                                if (btn === 'yes') {
                                    if (isSave) {
                                        Ext.getCmp('move-prev').setDisabled(false);
                                        Ext.getCmp('move-prev').show();
                                        layout[direction]();
                                        currentPanel++;
                                        if (!layout.getNext()) {
                                            Ext.getCmp('move-next').hide();
                                        } else {
                                            Ext.getCmp('move-next').setText(NEXT_MOVE);
                                        }
                                    }
                                }
                            })
                        }
                        else {
                            if (isSave) {
                                Ext.getCmp('move-prev').setDisabled(false);
                                Ext.getCmp('move-prev').show();
                                layout[direction]();
                                currentPanel++;
                                if (!layout.getNext()) {
                                    Ext.getCmp('move-next').hide();
                                } else {
                                    Ext.getCmp('move-next').setText(NEXT_MOVE);
                                }
                            }
                        }

                    } else {
                        if (Ext.getCmp('cost_percent').getValue() < 50) {
                            Ext.Msg.confirm("注意", "是否確定成本百分比小於50？", function (btn) {
                                if (btn === 'yes') {
                                    //編輯時若選中失格則判斷該廠商品牌下的所有商品是否都下架並且失格，若沒有則提示
                                    if (Ext.getCmp("vendor_status").getValue().vendor_status == 3) {
                                        Ext.Ajax.request({
                                            url: "/Vendor/GetOffGradeCount",
                                            method: 'post',
                                            async: false, //true為異步，false為同步
                                            params: {
                                                vendor_id: VendorID
                                            },
                                            success: function (form, action) {
                                                var result = Ext.decode(form.responseText);
                                                if (result.success) {
                                                    if (result.count != 0) {//該供應商下所有未失格商品的個數 0代表商品全部失格
                                                        isSave = false;
                                                        Ext.Msg.alert(INFORMATION, '該供應商下還有未失格的商品！');
                                                    }
                                                }
                                            }
                                        });
                                    }

                                    if (isSave) {
                                        //編輯時分步保存
                                        var InsertValues = "";
                                        var gdcontact = Ext.getCmp("gdcontact").store.data.items;

                                        for (var a = 0; a < gdcontact.length; a++) {
                                            var type = gdcontact[a].get("contact_type");
                                            var name = gdcontact[a].get("contact_name");
                                            var phone1 = gdcontact[a].get("contact_phone1");
                                            var phone2 = gdcontact[a].get("contact_phone2");
                                            var mobile = gdcontact[a].get("contact_mobile");
                                            var email = gdcontact[a].get("contact_email");

                                            InsertValues += type + ',' + name + ',' + phone1 + ',' + phone2 + ',' + mobile + ',' + email + '|';
                                        }
                                        formf.submit({
                                            url: '/Vendor/SaveVendor',
                                            params: {
                                                vendor_id: VendorID,
                                                delconnect: delconnect,
                                                vendor_code: Ext.htmlEncode(Ext.getCmp("vendor_code").getValue()),
                                                vendor_status: Ext.htmlEncode(Ext.getCmp("vendor_status").getValue().vendor_status),
                                                vendor_email: Ext.htmlEncode(Ext.getCmp("vendor_email").getValue()),
                                                vendor_password: Ext.htmlEncode(Ext.getCmp("vendor_password").getValue()),
                                                vendor_name_full: Ext.htmlEncode(Ext.getCmp("vendor_name_full").getValue()),
                                                vendor_name_simple: Ext.htmlEncode(Ext.getCmp("vendor_name_simple").getValue()),
                                                vendor_invoice: Ext.htmlEncode(Ext.getCmp("vendor_invoice").getValue()),
                                                company_phone: Ext.htmlEncode(Ext.getCmp("company_phone").getValue()),
                                                company_fax: Ext.htmlEncode(Ext.getCmp("company_fax").getValue()),
                                                company_person: Ext.htmlEncode(Ext.getCmp("company_person").getValue()),
                                                company_zip: Ext.htmlEncode(Ext.getCmp('company_zip').getValue()),
                                                company_address: Ext.htmlEncode(Ext.getCmp('company_address').getValue()),
                                                invoice_zip: Ext.htmlEncode(Ext.getCmp('invoice_zip').getValue()),
                                                invoice_address: Ext.htmlEncode(Ext.getCmp('invoice_address').getValue()),
                                                prod_cate: Ext.htmlEncode(Ext.getCmp('prod_cate').getValue()),
                                                buy_cate: Ext.htmlEncode(Ext.getCmp('buy_cate').getValue()),
                                                tax_type: Ext.htmlEncode(Ext.getCmp('tax_type').getValue()),
                                                erp_id: Ext.htmlEncode(Ext.getCmp("erp_id").getValue()),
                                                conactValues: InsertValues,
                                                cost_percent: Ext.htmlEncode(Ext.getCmp("cost_percent").getValue()),
                                                creditcard_1_percent: Ext.htmlEncode(Ext.getCmp("creditcard_1_percent").getValue()),
                                                creditcard_3_percent: Ext.htmlEncode(Ext.getCmp("creditcard_3_percent").getValue()),
                                                agreement_createdate: Ext.htmlEncode(Ext.getCmp("agreement_createdate").getRawValue()),
                                                agreement_start: Ext.htmlEncode(Ext.getCmp("agreement_start").getRawValue()),
                                                agreement_end: Ext.htmlEncode(Ext.getCmp("agreement_end").getRawValue()),
                                                checkout_type: Ext.htmlEncode(Ext.getCmp("checkout_type").getValue().checkout_type),
                                                checkout_other: Ext.htmlEncode(Ext.getCmp("checkout_other").getValue()),
                                                bank_code: Ext.htmlEncode(Ext.getCmp("bank_code").getValue()),
                                                bank_name: Ext.htmlEncode(Ext.getCmp("bank_name").getValue()),
                                                bank_number: Ext.htmlEncode(Ext.getCmp("bank_number").getValue()),
                                                bank_account: Ext.htmlEncode(Ext.getCmp("bank_account").getValue()),
                                                freight_low_limit: Ext.htmlEncode(Ext.getCmp("freight_low_limit").getValue()),
                                                freight_low_money: Ext.htmlEncode(Ext.getCmp("freight_low_money").getValue()),
                                                freight_return_low_money: Ext.htmlEncode(Ext.getCmp("freight_return_low_money").getValue()),
                                                freight_normal_limit: Ext.htmlEncode(Ext.getCmp("freight_normal_limit").getValue()),
                                                freight_normal_money: Ext.htmlEncode(Ext.getCmp("freight_normal_money").getValue()),
                                                freight_return_normal_money: Ext.htmlEncode(Ext.getCmp("freight_return_normal_money").getValue()),
                                                // assist: Ext.htmlEncode(Ext.getCmp("assist").getValue().assist),
                                                //  dispatch: Ext.htmlEncode(Ext.getCmp("dispatch").getValue().dispatch),
                                                // product_mode: Ext.htmlEncode(Ext.getCmp("product_mode").getValue().product_mode),
                                                procurement_days: Ext.htmlEncode(Ext.getCmp("procurement_days").getValue()),
                                                self_send_days: Ext.htmlEncode(Ext.getCmp("self_send_days").getValue()),
                                                stuff_ware_days: Ext.htmlEncode(Ext.getCmp("stuff_ware_days").getValue()),
                                                dispatch_days: Ext.htmlEncode(Ext.getCmp("dispatch_days").getValue()),
                                                pm: Ext.htmlEncode(Ext.getCmp("manage_name").getValue()),
                                                gigade_bunus_percent: Ext.htmlEncode(Ext.getCmp("gigade_bunus_percent").getValue()),
                                                vendor_note: Ext.htmlEncode(Ext.getCmp("vendor_note").getValue()),
                                                gigade_bunus_threshold: Ext.htmlEncode(Ext.getCmp("gigade_bunus_threshold").getValue()),
                                                gigade_vendor_type: Ext.htmlEncode(Ext.getCmp("vendor_type").getValue()) //供應商類型
                                            },
                                            success: function (form, action) {
                                                var result = Ext.decode(action.response.responseText);
                                                if (result.success) {
                                                    if (result.msg == 0) {
                                                        Ext.Msg.alert(INFORMATION, "公司email不能重複，請修改您的輸入 ");
                                                    } else {
                                                        Ext.getCmp('move-prev').setDisabled(false);
                                                        Ext.getCmp('move-prev').show();
                                                        layout[direction]();
                                                        currentPanel++;
                                                        if (!layout.getNext()) {
                                                            Ext.getCmp('move-next').hide();
                                                        } else {
                                                            //Ext.getCmp('move-next').setText(NEXT_MOVE);
                                                        }
                                                    }
                                                } else {
                                                    Ext.Msg.alert(INFORMATION, FAILURE);
                                                }
                                            },
                                            failure: function (form, action) {
                                                Ext.Msg.alert(INFORMATION, FAILURE);
                                            }
                                        });
                                    }
                                }

                            })
                        }
                        else {
                            //編輯時若選中失格則判斷該廠商品牌下的所有商品是否都下架並且失格，若沒有則提示
                            if (Ext.getCmp("vendor_status").getValue().vendor_status == 3) {
                                Ext.Ajax.request({
                                    url: "/Vendor/GetOffGradeCount",
                                    method: 'post',
                                    async: false, //true為異步，false為同步
                                    params: {
                                        vendor_id: VendorID
                                    },
                                    success: function (form, action) {
                                        var result = Ext.decode(form.responseText);
                                        if (result.success) {
                                            if (result.count != 0) {//該供應商下所有未失格商品的個數 0代表商品全部失格
                                                isSave = false;
                                                Ext.Msg.alert(INFORMATION, '該供應商下還有未失格的商品！');
                                            }
                                        }
                                    }
                                });
                            }

                            if (isSave) {
                                //編輯時分步保存
                                var InsertValues = "";
                                var gdcontact = Ext.getCmp("gdcontact").store.data.items;

                                for (var a = 0; a < gdcontact.length; a++) {
                                    var type = gdcontact[a].get("contact_type");
                                    var name = gdcontact[a].get("contact_name");
                                    var phone1 = gdcontact[a].get("contact_phone1");
                                    var phone2 = gdcontact[a].get("contact_phone2");
                                    var mobile = gdcontact[a].get("contact_mobile");
                                    var email = gdcontact[a].get("contact_email");

                                    InsertValues += type + ',' + name + ',' + phone1 + ',' + phone2 + ',' + mobile + ',' + email + '|';
                                }
                                formf.submit({
                                    url: '/Vendor/SaveVendor',
                                    params: {
                                        vendor_id: VendorID,
                                        delconnect: delconnect,
                                        vendor_code: Ext.htmlEncode(Ext.getCmp("vendor_code").getValue()),
                                        vendor_status: Ext.htmlEncode(Ext.getCmp("vendor_status").getValue().vendor_status),
                                        vendor_email: Ext.htmlEncode(Ext.getCmp("vendor_email").getValue()),
                                        vendor_password: Ext.htmlEncode(Ext.getCmp("vendor_password").getValue()),
                                        vendor_name_full: Ext.htmlEncode(Ext.getCmp("vendor_name_full").getValue()),
                                        vendor_name_simple: Ext.htmlEncode(Ext.getCmp("vendor_name_simple").getValue()),
                                        vendor_invoice: Ext.htmlEncode(Ext.getCmp("vendor_invoice").getValue()),
                                        company_phone: Ext.htmlEncode(Ext.getCmp("company_phone").getValue()),
                                        company_fax: Ext.htmlEncode(Ext.getCmp("company_fax").getValue()),
                                        company_person: Ext.htmlEncode(Ext.getCmp("company_person").getValue()),
                                        company_zip: Ext.htmlEncode(Ext.getCmp('company_zip').getValue()),
                                        company_address: Ext.htmlEncode(Ext.getCmp('company_address').getValue()),
                                        invoice_zip: Ext.htmlEncode(Ext.getCmp('invoice_zip').getValue()),
                                        invoice_address: Ext.htmlEncode(Ext.getCmp('invoice_address').getValue()),
                                        prod_cate: Ext.htmlEncode(Ext.getCmp('prod_cate').getValue()),
                                        buy_cate: Ext.htmlEncode(Ext.getCmp('buy_cate').getValue()),
                                        tax_type: Ext.htmlEncode(Ext.getCmp('tax_type').getValue()),
                                        erp_id: Ext.htmlEncode(Ext.getCmp("erp_id").getValue()),
                                        conactValues: InsertValues,
                                        cost_percent: Ext.htmlEncode(Ext.getCmp("cost_percent").getValue()),
                                        creditcard_1_percent: Ext.htmlEncode(Ext.getCmp("creditcard_1_percent").getValue()),
                                        creditcard_3_percent: Ext.htmlEncode(Ext.getCmp("creditcard_3_percent").getValue()),
                                        agreement_createdate: Ext.htmlEncode(Ext.getCmp("agreement_createdate").getRawValue()),
                                        agreement_start: Ext.htmlEncode(Ext.getCmp("agreement_start").getRawValue()),
                                        agreement_end: Ext.htmlEncode(Ext.getCmp("agreement_end").getRawValue()),
                                        checkout_type: Ext.htmlEncode(Ext.getCmp("checkout_type").getValue().checkout_type),
                                        checkout_other: Ext.htmlEncode(Ext.getCmp("checkout_other").getValue()),
                                        bank_code: Ext.htmlEncode(Ext.getCmp("bank_code").getValue()),
                                        bank_name: Ext.htmlEncode(Ext.getCmp("bank_name").getValue()),
                                        bank_number: Ext.htmlEncode(Ext.getCmp("bank_number").getValue()),
                                        bank_account: Ext.htmlEncode(Ext.getCmp("bank_account").getValue()),
                                        freight_low_limit: Ext.htmlEncode(Ext.getCmp("freight_low_limit").getValue()),
                                        freight_low_money: Ext.htmlEncode(Ext.getCmp("freight_low_money").getValue()),
                                        freight_return_low_money: Ext.htmlEncode(Ext.getCmp("freight_return_low_money").getValue()),
                                        freight_normal_limit: Ext.htmlEncode(Ext.getCmp("freight_normal_limit").getValue()),
                                        freight_normal_money: Ext.htmlEncode(Ext.getCmp("freight_normal_money").getValue()),
                                        freight_return_normal_money: Ext.htmlEncode(Ext.getCmp("freight_return_normal_money").getValue()),
                                        // assist: Ext.htmlEncode(Ext.getCmp("assist").getValue().assist),
                                        //  dispatch: Ext.htmlEncode(Ext.getCmp("dispatch").getValue().dispatch),
                                        // product_mode: Ext.htmlEncode(Ext.getCmp("product_mode").getValue().product_mode),
                                        procurement_days: Ext.htmlEncode(Ext.getCmp("procurement_days").getValue()),
                                        self_send_days: Ext.htmlEncode(Ext.getCmp("self_send_days").getValue()),
                                        stuff_ware_days: Ext.htmlEncode(Ext.getCmp("stuff_ware_days").getValue()),
                                        dispatch_days: Ext.htmlEncode(Ext.getCmp("dispatch_days").getValue()),
                                        pm: Ext.htmlEncode(Ext.getCmp("manage_name").getValue()),
                                        gigade_bunus_percent: Ext.htmlEncode(Ext.getCmp("gigade_bunus_percent").getValue()),
                                        vendor_note: Ext.htmlEncode(Ext.getCmp("vendor_note").getValue()),
                                        gigade_bunus_threshold: Ext.htmlEncode(Ext.getCmp("gigade_bunus_threshold").getValue()),
                                        gigade_vendor_type: Ext.htmlEncode(Ext.getCmp("vendor_type").getValue()) //供應商類型
                                    },
                                    success: function (form, action) {
                                        var result = Ext.decode(action.response.responseText);
                                        if (result.success) {
                                            if (result.msg == 0) {
                                                Ext.Msg.alert(INFORMATION, "公司email不能重複，請修改您的輸入 ");
                                            } else {
                                                Ext.getCmp('move-prev').setDisabled(false);
                                                Ext.getCmp('move-prev').show();
                                                layout[direction]();
                                                currentPanel++;
                                                if (!layout.getNext()) {
                                                    Ext.getCmp('move-next').hide();
                                                } else {
                                                    //Ext.getCmp('move-next').setText(NEXT_MOVE);
                                                }
                                            }
                                        } else {
                                            Ext.Msg.alert(INFORMATION, FAILURE);
                                        }
                                    },
                                    failure: function (form, action) {
                                        Ext.Msg.alert(INFORMATION, FAILURE);
                                    }
                                });
                            }
                        }

                    }
                }

            } else {
                var forms = secondForm.getForm();
                if (forms.isValid()) {
                    if (!rowID) {
                        Ext.getCmp('move-prev').setDisabled(false);
                        Ext.getCmp('move-prev').show();
                        layout[direction]();
                        currentPanel++;
                        Ext.getCmp('move-next').hide();
                    }
                        //編輯時分步保存
                    else {
                        var InsertValues = "";
                        var gdcontact = Ext.getCmp("gdcontact").store.data.items;

                        for (var a = 0; a < gdcontact.length; a++) {
                            var type = gdcontact[a].get("contact_type");
                            var name = gdcontact[a].get("contact_name");
                            var phone1 = gdcontact[a].get("contact_phone1");
                            var phone2 = gdcontact[a].get("contact_phone2");
                            var mobile = gdcontact[a].get("contact_mobile");
                            var email = gdcontact[a].get("contact_email");
                            InsertValues += type + ',' + name + ',' + phone1 + ',' + phone2 + ',' + mobile + ',' + email + '|';
                        }
                        forms.submit({
                            url: '/Vendor/SaveVendor',
                            params: {
                                vendor_id: VendorID,
                                delconnect: delconnect,

                                vendor_code: Ext.htmlEncode(Ext.getCmp("vendor_code").getValue()),
                                vendor_status: Ext.htmlEncode(Ext.getCmp("vendor_status").getValue().vendor_status),
                                vendor_email: Ext.htmlEncode(Ext.getCmp("vendor_email").getValue()),
                                vendor_password: Ext.htmlEncode(Ext.getCmp("vendor_password").getValue()),
                                vendor_name_full: Ext.htmlEncode(Ext.getCmp("vendor_name_full").getValue()),
                                vendor_name_simple: Ext.htmlEncode(Ext.getCmp("vendor_name_simple").getValue()),
                                vendor_invoice: Ext.htmlEncode(Ext.getCmp("vendor_invoice").getValue()),
                                company_phone: Ext.htmlEncode(Ext.getCmp("company_phone").getValue()),
                                company_fax: Ext.htmlEncode(Ext.getCmp("company_fax").getValue()),
                                company_person: Ext.htmlEncode(Ext.getCmp("company_person").getValue()),
                                company_zip: Ext.htmlEncode(Ext.getCmp('company_zip').getValue()),
                                company_address: Ext.htmlEncode(Ext.getCmp('company_address').getValue()),
                                invoice_zip: Ext.htmlEncode(Ext.getCmp('invoice_zip').getValue()),
                                invoice_address: Ext.htmlEncode(Ext.getCmp('invoice_address').getValue()),
                                prod_cate: Ext.htmlEncode(Ext.getCmp('prod_cate').getValue()),
                                buy_cate: Ext.htmlEncode(Ext.getCmp('buy_cate').getValue()),
                                tax_type: Ext.htmlEncode(Ext.getCmp('tax_type').getValue()),
                                erp_id: Ext.htmlEncode(Ext.getCmp("erp_id").getValue()),
                                conactValues: InsertValues,
                                cost_percent: Ext.htmlEncode(Ext.getCmp("cost_percent").getValue()),
                                creditcard_1_percent: Ext.htmlEncode(Ext.getCmp("creditcard_1_percent").getValue()),
                                creditcard_3_percent: Ext.htmlEncode(Ext.getCmp("creditcard_3_percent").getValue()),
                                agreement_createdate: Ext.htmlEncode(Ext.getCmp("agreement_createdate").getRawValue()),
                                agreement_start: Ext.htmlEncode(Ext.getCmp("agreement_start").getRawValue()),
                                agreement_end: Ext.htmlEncode(Ext.getCmp("agreement_end").getRawValue()),
                                checkout_type: Ext.htmlEncode(Ext.getCmp("checkout_type").getValue().checkout_type),
                                checkout_other: Ext.htmlEncode(Ext.getCmp("checkout_other").getValue()),
                                bank_code: Ext.htmlEncode(Ext.getCmp("bank_code").getValue()),
                                bank_name: Ext.htmlEncode(Ext.getCmp("bank_name").getValue()),
                                bank_number: Ext.htmlEncode(Ext.getCmp("bank_number").getValue()),
                                bank_account: Ext.htmlEncode(Ext.getCmp("bank_account").getValue()),
                                freight_low_limit: Ext.htmlEncode(Ext.getCmp("freight_low_limit").getValue()),
                                freight_low_money: Ext.htmlEncode(Ext.getCmp("freight_low_money").getValue()),
                                freight_return_low_money: Ext.htmlEncode(Ext.getCmp("freight_return_low_money").getValue()),
                                freight_normal_limit: Ext.htmlEncode(Ext.getCmp("freight_normal_limit").getValue()),
                                freight_normal_money: Ext.htmlEncode(Ext.getCmp("freight_normal_money").getValue()),
                                freight_return_normal_money: Ext.htmlEncode(Ext.getCmp("freight_return_normal_money").getValue()),
                                // assist: Ext.htmlEncode(Ext.getCmp("assist").getValue().assist),
                                //  dispatch: Ext.htmlEncode(Ext.getCmp("dispatch").getValue().dispatch),
                                // product_mode: Ext.htmlEncode(Ext.getCmp("product_mode").getValue().product_mode),
                                procurement_days: Ext.htmlEncode(Ext.getCmp("procurement_days").getValue()),
                                self_send_days: Ext.htmlEncode(Ext.getCmp("self_send_days").getValue()),
                                stuff_ware_days: Ext.htmlEncode(Ext.getCmp("stuff_ware_days").getValue()),
                                dispatch_days: Ext.htmlEncode(Ext.getCmp("dispatch_days").getValue()),
                                pm: Ext.htmlEncode(Ext.getCmp("manage_name").getValue()),
                                gigade_bunus_percent: Ext.htmlEncode(Ext.getCmp("gigade_bunus_percent").getValue()),
                                vendor_note: Ext.htmlEncode(Ext.getCmp("vendor_note").getValue()),
                                gigade_bunus_threshold: Ext.htmlEncode(Ext.getCmp("gigade_bunus_threshold").getValue()),
                                gigade_vendor_type: Ext.htmlEncode(Ext.getCmp("vendor_type").getValue()) //供應商類型
                            },
                            success: function (form, action) {
                                var result = Ext.decode(action.response.responseText);
                                if (result.success) {
                                    if (result.msg == 0) {
                                        Ext.Msg.alert(INFORMATION, "公司email不能重複，請修改您的輸入 ");
                                    } else {
                                        Ext.getCmp('move-prev').setDisabled(false);
                                        Ext.getCmp('move-prev').show();
                                        layout[direction]();
                                        currentPanel++;
                                        Ext.getCmp('move-next').hide();
                                    }
                                } else {
                                    Ext.Msg.alert(INFORMATION, FAILURE);
                                }
                            },
                            failure: function (form, action) {
                                Ext.Msg.alert(INFORMATION, FAILURE);
                            }
                        });
                    }
                }
            }
        } else {
            Ext.getCmp('move-prev').setDisabled(false);
            layout[direction]();
            currentPanel--;
            if (currentPanel === 0) {
                Ext.getCmp('move-prev').hide();
            } else {
                Ext.getCmp('move-prev').show();
            }
            Ext.getCmp('move-next').show();
        }
        Ext.getCmp('move-prev').setDisabled(!layout.getPrev());
    };

    var firstForm = Ext.widget('form', {
        id: 'editFrm1',
        plain: true,
        frame: true,
        defaultType: 'textfield',
        layout: 'anchor',
        //buttonAlign: 'right',
        //buttons: [{
        //    text: SAVE,
        //    hidden: rowID ? false : true,
        //    handler: function () {
        //        alert("s");
        //    }
        //}],
        items: [
            {
                xtype: 'fieldcontainer',
                combineErrors: true,
                id: 'editcon',
                layout: 'hbox',
                //hidden: true,
                defaults: {
                    width: 210,
                    labelWidth: 80
                },
                items: [
                    {
                        fieldLabel: '供應商編號',
                        xtype: 'displayfield',
                        id: 'vendor_id',
                        //hidden: false,
                        name: 'vendor_id',
                        allowBlank: false
                    },
                    {
                        fieldLabel: '供應商編碼',
                        xtype: 'displayfield',
                        id: 'vendor_code',
                        // hidden: true,
                        margin: '0 8 0 20',
                        name: 'vendor_code',
                        allowBlank: false
                    }
                ]
            },
            {
                xtype: 'fieldcontainer',
                combineErrors: true,
                layout: 'hbox',
                defaults: {
                    width: 210,
                    labelWidth: 80
                },
                items: [
                    {
                        fieldLabel: '供應商名稱',
                        xtype: 'textfield',
                        id: 'vendor_name_full',
                        name: 'vendor_name_full',
                        //  hidden: true,
                        allowBlank: false
                    },
                    {
                        fieldLabel: '供應商簡稱',
                        xtype: 'textfield',
                        id: 'vendor_name_simple',
                        margin: '0 8 0 20',
                        name: 'vendor_name_simple',
                        // hidden: true,
                        allowBlank: false
                    }
                ]
            },
            {
                xtype: 'fieldcontainer',
                combineErrors: true,
                layout: 'hbox',
                defaults: {
                    width: 210,
                    labelWidth: 80
                },
                items: [
                    {
                        fieldLabel: '統一編號',
                        xtype: 'textfield',
                        id: 'vendor_invoice',
                        name: 'vendor_invoice',
                        allowBlank: false
                    },
                    {
                        fieldLabel: '公司Email',
                        xtype: 'textfield',
                        id: 'vendor_email',
                        margin: '0 8 0 20',
                        name: 'vendor_email',
                        allowBlank: false,
                        vtype: 'email',
                        //regex:/^(")?(?:[^\."])(?:(?:[\.])?(?:[a-z0-9\-!#$%&'*+/=?^_`{|}~]))*\1@([a-z0-9][\-a-z0-9]*\.){1,5}([a-z]){2,6}$/,
                        regex: /^([a-z0-9_\.-]+)@([\da-z\.-]+)\.([a-z\.]{2,6})$/,
                        invalidText: 'E-Mail格式必須小寫'

                    }
                ]
            },
            {
                xtype: 'fieldcontainer',
                combineErrors: false,
                id: 'vendorsec',
                layout: 'hbox',
                defaults: {
                    width: 210,
                    labelWidth: 80
                },
                items: [
                    {
                        xtype: 'radiogroup',
                        hidden: false,
                        id: 'vendor_status',
                        name: 'vendor_status',
                        fieldLabel: '狀態',
                        // hidden: true,
                        colName: 'vendor_status',
                        defaults: {
                            name: 'vendor_status',
                            margin: '0 8 0 0'
                        },
                        columns: 3,
                        vertical: true,
                        items: [
                            {
                                boxLabel: '啟用', id: 'active', inputValue: '1', checked: true,
                                listeners: {
                                    change: function (radio, newValue, oldValue) {
                                        if (rowID) {
                                            var thac = Ext.getCmp("thnactive");
                                            if (newValue) {
                                                thac.hide();
                                            }
                                        }
                                    }
                                }
                            },
                            {
                                boxLabel: '停用', id: 'nonactive', inputValue: '2',
                                listeners: {
                                    change: function (radio, newValue, oldValue) {
                                        if (rowID) {
                                            var thac = Ext.getCmp("thnactive");
                                            if (newValue) {
                                                thac.show();
                                            }

                                        }
                                    }
                                }

                            },
                               {
                                   boxLabel: '失格', hidden: true, id: 'thnactive', inputValue: '3',
                                   listeners: {
                                       change: function (radio, newValue, oldValue) {

                                           if (newValue) {
                                               if (row.data.vendor_status != 3) {
                                                   Ext.MessageBox.confirm(CONFIRM, "請確認是否失格？", function (btn) {
                                                       if (btn === "yes") {
                                                           Ext.Ajax.request({
                                                               url: "/Vendor/GetOffGradeCount",
                                                               method: 'post',
                                                               async: false, //true為異步，false為同步
                                                               params: {
                                                                   vendor_id: VendorID
                                                               },
                                                               success: function (form, action) {
                                                                   var result = Ext.decode(form.responseText);
                                                                   if (result.success) {
                                                                       if (result.count != 0) {//該供應商下所有未失格商品的個數 0代表商品全部失格
                                                                           Ext.Msg.alert(INFORMATION, '該供應商下還有未失格的商品！');
                                                                           switch (row.data.vendor_status) {
                                                                               case 1:
                                                                                   Ext.getCmp("active").setValue(true);
                                                                                   break;
                                                                               case 2:
                                                                                   Ext.getCmp("nonactive").setValue(true);
                                                                                   break;
                                                                               case 3:
                                                                                   Ext.getCmp("thnactive").setValue(true);
                                                                                   break;

                                                                           }
                                                                       }
                                                                   }
                                                               }
                                                           });
                                                       }
                                                       else {
                                                           switch (row.data.vendor_status) {
                                                               case 1:
                                                                   Ext.getCmp("active").setValue(true);
                                                                   break;
                                                               case 2:
                                                                   Ext.getCmp("nonactive").setValue(true);
                                                                   break;
                                                               case 3:
                                                                   Ext.getCmp("thnactive").setValue(true);
                                                                   break;

                                                           }

                                                       }
                                                   });
                                               }
                                           }
                                       }
                                   }
                               }

                        ]
                    },
                    {
                        fieldLabel: '密碼',
                        xtype: 'textfield',
                        id: 'vendor_password',
                        name: 'vendor_password',
                        hidden: rowID ? true : false,
                        //allowBlank: rowID ? true : false,
                        margin: '0 8 0 20',
                        inputType: 'password'
                    }
                ]
            },
            {
                xtype: 'fieldcontainer',
                combineErrors: true,
                layout: 'hbox',
                defaults: {
                    width: 210,
                    labelWidth: 80
                },
                items: [
                    {
                        fieldLabel: '公司電話',
                        xtype: 'textfield',
                        id: 'company_phone',
                        name: 'company_phone',
                        allowBlank: false
                        //,
                        //regex: /^(00886-?)?(\d{2,4})-?(\d{3}\d?)-?(\d{4})(-?\d{3,4})?$/
                    },
                    {
                        fieldLabel: '公司傳真',
                        xtype: 'textfield',
                        margin: '0 8 0 20',
                        id: 'company_fax',
                        name: 'company_fax'
                        //,
                        //regex: /^(00886-?)?(\d{2,4})-?(\d{3}\d?)-?(\d{4})(-?\d{3,4})?$/
                    }
                ]
            }, {
                xtype: 'fieldcontainer',
                combineErrors: true,
                layout: 'hbox',
                defaults: {
                    width: 210,
                    labelWidth: 80
                },
                items: [
                {
                    fieldLabel: '公司負責人',
                    xtype: 'textfield',
                    id: 'company_person',
                    name: 'company_person',
                    allowBlank: false,
                    width: 210,
                    labelWidth: 80
                }, {
                    xtype: 'combobox', //Wibset
                    allowBlank: false,
                    editable: false,
                    fieldLabel: "供應商類型",
                    multiSelect: true, //支持多選
                    hidden: false,
                    margin: '0 8 0 20',
                    id: 'vendor_type',
                    name: 'vendor_type_name',
                    hiddenName: 'vendortype',
                    //colName: 'vendor_type_name',
                    store: VendorTypeStore,
                    displayField: 'vendor_type_name',
                    valueField: 'vendor_type',
                    queryMode: 'local',
                    typeAhead: true,
                    forceSelection: false,
                    emptyText: '請選擇',
                    listeners: {
                        select: function (combo, records, eOpts) {

                        }
                    }
                }
                ]
            },
            {
                xtype: 'fieldcontainer',
                layout: 'hbox',
                width: 500,
                defaults: {
                    hideLabel: true
                },
                items: [
                    {
                        xtype: 'displayfield',
                        value: '公司地址:',
                        //id: 'company_address',
                        width: 80
                    },
                    //{
                    //    xtype: 'combobox',
                    //    name: 'company_zip',
                    //    id: 'company_zip',
                    //    editable: false,
                    //    store: ZipStore,
                    //    queryMode: 'local',
                    //    submitValue: true,
                    //    displayField: 'middle',
                    //    valueField: 'zipcode',
                    //    typeAhead: true,
                    //    forceSelection: false,
                    //    value: ''
                    //},
                     {
                         xtype: 'combo',
                         id: 'cob_ccity_a',
                         name: 'cob_ccity_a',
                         queryMode: 'local',
                         displayField: 'big',
                         valueField: 'bigcode',
                         store: AreaStore,
                         editable: false,
                         lastQuery: '',
                         allowBlank: true,
                         margins: '0 0 0 5',
                         width: 70,
                         listeners: {
                             "select": function (combo, record) {
                                 var c = Ext.getCmp("cob_ccity_c");
                                 var z = Ext.getCmp("company_zip");
                                 c.clearValue();
                                 z.clearValue();
                                 CityStore.removeAll();
                                 ZipStore.removeAll();
                                 comArea = true;
                             }
                         }
                     },
                    {
                        xtype: 'combo',
                        id: 'cob_ccity_c',
                        name: 'cob_ccity_c',
                        queryMode: 'local',
                        displayField: 'middle',
                        valueField: 'middlecode',
                        store: CityStore,
                        editable: false,
                        lastQuery: '',
                        allowBlank: true,
                        margins: '0 0 0 5',
                        width: 70,
                        listeners: {
                            beforequery: function (qe) {
                                if (comArea) {
                                    delete qe.combo.lastQuery;
                                    CityStore.load({
                                        params: {
                                            topValue: Ext.getCmp("cob_ccity_a").getValue(),
                                            topText: Ext.getCmp("cob_ccity_a").rawValue
                                        }
                                    });
                                    comArea = false;
                                }
                            },
                            "select": function (combo, record) {
                                var z = Ext.getCmp("company_zip");
                                z.clearValue();
                                ZipStore.removeAll();
                                comCity = true;
                            }
                        }
                    },
                    {
                        xtype: 'combo',
                        name: 'company_zip',
                        id: 'company_zip',
                        queryMode: 'local',
                        displayField: 'small',
                        valueField: 'zipcode',
                        lastQuery: '',
                        store: ZipStore,
                        editable: false,
                        typeAhead: true,
                        allowBlank: true,
                        width: 90,
                        margins: '0 5 0 5',
                        listeners: {
                            beforequery: function (qe) {
                                if (comCity) {
                                    delete qe.combo.lastQuery;
                                    ZipStore.load({
                                        params: {
                                            topValue: Ext.getCmp("cob_ccity_c").getValue(),
                                            topText: Ext.getCmp("cob_ccity_c").rawValue
                                        }
                                    });
                                    comCity = false;
                                }
                            }
                        }
                    },
                    {
                        id: 'company_address',
                        xtype: 'textfield',
                        width: 140,
                        name: 'company_address'
                    }
                ]
            },
            {

                xtype: 'fieldcontainer',
                layout: 'hbox',
                width: 500,
                defaults: {
                    hideLabel: true
                },
                items: [
                  {
                      xtype: 'checkbox',
                      boxLabel: '發票地址同公司地址',
                      id: 'same_adress',
                      name: 'same_adress',
                      inputValue: '0',
                      listeners: {
                          change: function (checkbox, newValue, oldValue) {
                              var cob_ccity_ai = Ext.getCmp('cob_ccity_ai');
                              var cob_ccity_i = Ext.getCmp('cob_ccity_i');
                              var invoice_zip = Ext.getCmp('invoice_zip');
                              var invoice_address = Ext.getCmp('invoice_address');
                              if (newValue) {
                                  InvoiceZipStore.removeAll();
                                  InvoiceAreaStore.load({
                                      callback: function () {
                                          Ext.getCmp("cob_ccity_ai").setValue(Ext.getCmp('cob_ccity_a').getValue());
                                          InvoiceCityStore.load({
                                              params: {
                                                  topValue: Ext.getCmp('cob_ccity_a').getValue()
                                              },
                                              callback: function () {
                                                  Ext.getCmp("cob_ccity_i").setValue(Ext.getCmp('cob_ccity_c').getValue());
                                                  InvoiceZipStore.load({
                                                      params: {
                                                          topValue: Ext.getCmp("cob_ccity_i").getValue(),
                                                          topText: Ext.getCmp("cob_ccity_i").rawValue
                                                      },
                                                      callback: function () {
                                                          Ext.getCmp("invoice_zip").setValue(Ext.getCmp('company_zip').getValue());
                                                      }
                                                  });
                                              }
                                          });
                                      }
                                  });

                                  invoice_address.setValue(Ext.getCmp('company_address').getValue());



                              }
                              else {
                                  cob_ccity_ai.setValue("");
                                  cob_ccity_i.setValue("");
                                  invoice_zip.setValue("");
                                  invoice_address.setValue("");
                              }

                          }
                      }

                  }]
            },
            {

                xtype: 'fieldcontainer',
                layout: 'hbox',
                width: 500,
                defaults: {
                    hideLabel: true
                },
                items: [
                    {
                        xtype: 'displayfield',
                        value: '發票地址:',
                        //id: 'invoice_address',
                        width: 80
                    },

                      {
                          xtype: 'combo',
                          id: 'cob_ccity_ai',
                          name: 'cob_ccity_ai',
                          queryMode: 'local',
                          displayField: 'big',
                          valueField: 'bigcode',
                          store: InvoiceAreaStore,
                          editable: false,
                          lastQuery: '',
                          allowBlank: true,
                          margins: '0 0 0 5',
                          width: 70,
                          defaultListConfig: {
                              loadMask: false
                          },
                          listeners: {
                              "select": function (combo, record) {
                                  var c = Ext.getCmp("cob_ccity_i");
                                  var z = Ext.getCmp("invoice_zip");
                                  c.clearValue();
                                  z.clearValue();
                                  InvoiceCityStore.removeAll();
                                  InvoiceZipStore.removeAll();
                                  comArea_i = true;
                              }
                          }
                      },
                    {
                        xtype: 'combo',
                        id: 'cob_ccity_i',
                        queryMode: 'local',
                        displayField: 'middle',
                        valueField: 'middlecode',
                        lastQuery: '',
                        store: InvoiceCityStore,
                        editable: false,
                        allowBlank: true,
                        margins: '0 0 0 5',
                        width: 70,
                        listeners: {
                            beforequery: function (qe) {
                                if (comArea_i) {
                                    delete qe.combo.lastQuery;
                                    InvoiceCityStore.load({
                                        params: {
                                            topValue: Ext.getCmp("cob_ccity_ai").getValue(),
                                            topText: Ext.getCmp("cob_ccity_ai").rawValue
                                        }
                                    });
                                    comArea_i = false;
                                }
                            },
                            "select": function (combo, record) {
                                var z = Ext.getCmp("invoice_zip");
                                z.clearValue();
                                InvoiceZipStore.removeAll();
                                comCity_i = true;
                            }
                        }
                    },
                    {
                        xtype: 'combo',
                        name: 'invoice_zip',
                        id: 'invoice_zip',
                        queryMode: 'local',
                        displayField: 'small',
                        valueField: 'zipcode',
                        lastQuery: '',
                        store: InvoiceZipStore,
                        editable: false,
                        typeAhead: true,
                        allowBlank: true,
                        width: 90,
                        margins: '0 5 0 5',
                        listeners: {
                            beforequery: function (qe) {
                                if (comCity_i) {
                                    delete qe.combo.lastQuery;
                                    InvoiceZipStore.load({
                                        params: {
                                            topValue: Ext.getCmp("cob_ccity_i").getValue(),
                                            topText: Ext.getCmp("cob_ccity_i").rawValue
                                        }
                                    });
                                    comCity_i = false;
                                }
                            }
                        }
                    },
                    {
                        id: 'invoice_address',
                        xtype: 'textfield',
                        width: 140,
                        name: 'invoice_address'
                    }
                ]
            },
            {
                xtype: 'fieldcontainer',
                combineErrors: true,
                layout: 'hbox',
                items: [
                    {
                        xtype: 'numberfield',
                        name: 'cost_percent',
                        id: 'cost_percent',
                        width: 170,
                        labelWidth: 80,
                        allowDecimals: false,
                        fieldLabel: '成本',
                        allowBlank: false,
                        minValue: 0,
                        maxValue: 100
                    },
                    {
                        xtype: 'displayfield',
                        value: '百分比'
                    }
                ]
            },
            {
                xtype: 'fieldcontainer',
                combineErrors: true,
                layout: 'hbox',
                defaults: {
                    width: 210,
                    labelWidth: 80
                },
                items: [
                    {
                        fieldLabel: '採購天數',
                        xtype: 'numberfield',
                        minValue: 0,
                        value: 0,
                        id: 'procurement_days',
                        name: 'procurement_days'
                    },
                    {
                        fieldLabel: '自出出貨天數',
                        xtype: 'numberfield',
                        id: 'self_send_days',
                        margin: '0 8 0 20',
                        minValue: 0,
                        value: 0,
                        name: 'self_send_days'
                    }
                ]
            },
            {
                xtype: 'fieldcontainer',
                combineErrors: true,
                layout: 'hbox',
                defaults: {
                    width: 210,
                    labelWidth: 80
                },
                items: [
                    {
                        fieldLabel: '寄倉出貨天數',
                        xtype: 'numberfield',
                        id: 'stuff_ware_days',
                        minValue: 0,
                        value: 0,
                        name: 'stuff_ware_days'
                    },
                    {
                        fieldLabel: '調度出貨天數',
                        xtype: 'numberfield',
                        id: 'dispatch_days',
                        margin: '0 8 0 20',
                        value: 0,
                        name: 'dispatch_days',
                        minValue: 0
                    }
                ]
            }

            //{
            //    xtype: 'radiogroup',
            //    hidden: false,
            //    id: 'assist',
            //    name: 'assist',
            //    fieldLabel: '出貨模式',
            //    colName: 'assist',
            //    defaults: {
            //        name: 'assist',
            //        margin: '5 8 0 0'
            //    },
            //    width: 400,
            //    lableWidth: 70,
            //    columns: 2,
            //    vertical: true,
            //    items: [
            //      { boxLabel: '廠商自行管理', id: 'cs_self', inputValue: '0', checked: true },
            //      { boxLabel: '協助轉單', id: 'help', inputValue: '1' }
            //    ]
            //},
            //{
            //    xtype: 'radiogroup',
            //    hidden: false,
            //    id: 'dispatch',
            //    name: 'dispatch',
            //    width: 400,
            //    lableWidth: 70,
            //    fieldLabel: '調度倉模式',
            //    colName: 'dispatch',
            //    defaults: {
            //        name: 'dispatch',
            //        margin: '5 8 0 0'
            //    },
            //    columns: 2,
            //    vertical: true,
            //    items: [
            //      { boxLabel: '自行出貨', id: 'self', inputValue: '0', checked: true },
            //      { boxLabel: '進調度倉', id: 'diaoducang', inputValue: '1' }
            //    ]
            //},
            //{
            //         xtype: 'radiogroup',
            //         hidden: false,
            //         id: 'product_mode',
            //         name: 'product_mode',
            //         fieldLabel: '寄倉模式',
            //         colName: 'product_mode',
            //         defaults: {
            //             name: 'product_mode',
            //             margin: '  8 0 0'
            //         },
            //         width: 400,
            //         lableWidth: 70,
            //         columns: 2,
            //         vertical: true,
            //         items: [
            //      { boxLabel: '非寄倉', id: 'no', inputValue: '1', checked: true },
            //      { boxLabel: '寄倉', id: 'yes', inputValue: '2' }
            //         ]
            //     },
            //{
            //    xtype: 'displayfield',
            //    value: '  此欄位只提供會計備註用，不會異動到商品寄倉選項'
            //}
        ]
    });

    var secondForm = Ext.widget('form', {
        id: 'editFrm3',
        plain: true,
        frame: true,
        defaultType: 'textfield',
        layout: 'anchor',
        items: [
            {
                xtype: 'fieldcontainer',
                combineErrors: true,
                layout: 'hbox',
                items: [
                    {
                        xtype: 'numberfield',
                        name: 'creditcard_1_percent',
                        id: 'creditcard_1_percent',
                        labelWidth: 120,
                        allowDecimals: false,
                        fieldLabel: '信用卡一期手續費',
                        minValue: 0,
                        maxValue: 100
                    },
                    {
                        xtype: 'displayfield',
                        width: 40,
                        value: '百分比'
                    }
                ]
            },
            {
                xtype: 'fieldcontainer',
                combineErrors: true,
                layout: 'hbox',
                items: [
                     {
                         xtype: 'numberfield',
                         name: 'creditcard_3_percent',
                         id: 'creditcard_3_percent',
                         labelWidth: 120,
                         allowDecimals: false,
                         fieldLabel: '信用卡三期手續費',
                         minValue: 0,
                         maxValue: 10
                         //,
                         //regex: /^(([0-9]+\.[0-9]*[1-9][0-9]*)|([0-9]*[1-9][0-9]*\.[0-9]+)|([0-9]*[1-9][0-9]*))$/
                     },
                     {
                         xtype: 'displayfield',
                         width: 40,
                         value: '百分比'
                     }
                ]
            },
            {
                xtype: "datefield",
                id: 'agreement_createdate',
                name: 'agreement_createdate',
                format: 'Y-m-d',
                labelWidth: 120,
                fieldLabel: '合約簽訂日期',
                allowBlank: false,
                submitValue: true,
                value: Tomorrow()
            },
            {
                xtype: 'fieldcontainer',
                combineErrors: true,
                layout: 'hbox',
                labelWidth: 120,
                fieldLabel: '合約期間',
                items: [
                    {
                        xtype: "datefield",
                        id: 'agreement_start',
                        name: 'agreement_start',
                        format: 'Y-m-d',
                        allowBlank: false,
                        //editable: false,
                        submitValue: true,
                        value: Tomorrow(),
                        listeners: {
                            select: function (a, b, c) {
                                var start = Ext.getCmp("agreement_start");
                                var end = Ext.getCmp("agreement_end");
                                var s_date = new Date(start.getValue());
                                end.setValue(new Date(s_date.setFullYear(s_date.getFullYear() + 1)));
                            }
                        }
                    },
                    {
                        xtype: 'displayfield',
                        value: '~'
                    },
                    {
                        xtype: "datefield",
                        format: 'Y-m-d',
                        id: 'agreement_end',
                        // editable: false,
                        name: 'agreement_end',
                        allowBlank: false,
                        submitValue: true,
                        value: new Date(Tomorrow().setFullYear(Tomorrow().getFullYear() + 1)),
                        listeners: {
                            select: function (a, b, c) {
                                var start = Ext.getCmp("agreement_start");
                                var end = Ext.getCmp("agreement_end");
                                if (end.getValue() < start.getValue()) {
                                    Ext.Msg.alert(INFORMATION, '開始時間不能大於結束時間');
                                    end.setValue(new Date(Tomorrow().setFullYear(Tomorrow().getFullYear() + 1)));
                                }
                            }
                        }
                    }
                ]
            },
            {
                xtype: 'fieldcontainer',
                combineErrors: false,
                layout: 'hbox',
                labelWidth: 120,
                fieldLabel: '供應商結賬方式',
                items: [
                    {
                        xtype: 'radiogroup',
                        hidden: false,
                        id: 'checkout_type',
                        name: 'checkout_type',
                        width: 200,
                        lableWidth: 70,
                        colName: 'checkout_type',
                        defaults: {
                            name: 'checkout_type',
                            margin: '5 8 0 0'
                        },
                        columns: 3,
                        vertical: true,
                        items: [
                            {
                                boxLabel: '月結', id: '1', inputValue: '1', checked: true,
                                listeners: {
                                    change: function (radio, newValue, oldValue) {
                                        if (newValue) {
                                            Ext.getCmp('checkout_other').setValue(" ");
                                            Ext.getCmp('checkout_other').setDisabled(true);
                                            Ext.getCmp('checkout_other').allowBlank = true;
                                            Ext.getCmp('checkout_other').setValue("");
                                        }
                                    }
                                }
                            },
                            {
                                boxLabel: '半月結', id: '2', inputValue: '2',
                                listeners: {
                                    change: function (radio, newValue, oldValue) {
                                        if (newValue) {
                                            Ext.getCmp('checkout_other').setValue(" ");
                                            Ext.getCmp('checkout_other').setDisabled(true);
                                            Ext.getCmp('checkout_other').allowBlank = true;
                                            Ext.getCmp('checkout_other').setValue("");
                                        }
                                    }
                                }
                            },
                            {
                                boxLabel: '其他', id: '3', inputValue: '3', width: 60,
                                listeners: {
                                    change: function (radio, newValue, oldValue) {
                                        if (newValue) {
                                            Ext.getCmp('checkout_other').setDisabled(false);
                                            Ext.getCmp('checkout_other').allowBlank = false;
                                        }
                                    }
                                }
                            }
                        ]
                    },
                    {
                        xtype: 'textfield',
                        id: 'checkout_other',
                        name: 'checkout_other',
                        width: 120,
                        margin: '1 0 0 1',
                        disabled: true
                    }
                ]
            },
            {
                xtype: 'fieldcontainer',
                combineErrors: true,
                layout: 'hbox',
                defaults: {
                    width: 210,
                    labelWidth: 70
                },
                items: [
                    {
                        fieldLabel: '銀行代碼',
                        xtype: 'textfield',
                        id: 'bank_code',
                        name: 'bank_code',
                        allowBlank: false
                        //,
                        //regex: /^[-+]?([1-9]\d*|0)$/
                    },
                    {
                        fieldLabel: '銀行名稱',
                        xtype: 'textfield',
                        margin: '0 8 0 20',
                        id: 'bank_name',
                        name: 'bank_name',
                        allowBlank: false
                    }
                ]
            },
            {
                xtype: 'fieldcontainer',
                combineErrors: true,
                layout: 'hbox',
                defaults: {
                    width: 210,
                    labelWidth: 70
                },
                items: [
                    {
                        fieldLabel: '銀行帳號',
                        xtype: 'textfield',
                        id: 'bank_number',
                        name: 'bank_number',
                        allowBlank: false
                        //,
                        //regex: /^[-+]?([1-9]\d*|0)$/
                    },
                    {
                        fieldLabel: '銀行戶名',
                        xtype: 'textfield',
                        margin: '0 8 0 20',
                        id: 'bank_account',
                        name: 'bank_account',
                        allowBlank: false
                    }
                ]
            },
            {
                xtype: 'fieldcontainer',
                combineErrors: true,
                layout: 'hbox',
                labelWidth: 70,
                fieldLabel: '常溫商品',
                items: [
                    {
                        xtype: 'displayfield',
                        value: '運費'
                    },
                    {
                        xtype: 'textfield',
                        id: 'freight_normal_money',
                        name: 'freight_normal_money',
                        regex: /^([1-9]\d*|0)$/,
                        width: 40
                    },
                    {
                        xtype: 'displayfield',
                        value: '元，運費'
                    },
                    {
                        xtype: 'textfield',
                        id: 'freight_normal_limit',
                        name: 'freight_normal_limit',
                        regex: /^([1-9]\d*|0)$/,
                        width: 40
                    },
                    {
                        xtype: 'displayfield',
                        value: '元(含)以上免運費，逆物流運費'
                    },
                    {
                        xtype: 'textfield',
                        id: 'freight_return_normal_money',
                        name: 'freight_return_normal_money',
                        regex: /^([1-9]\d*|0)$/,
                        width: 40
                    },
                    {
                        xtype: 'displayfield',
                        value: '元'
                    }
                ]
            },
            {
                xtype: 'fieldcontainer',
                combineErrors: true,
                layout: 'hbox',
                labelWidth: 70,
                fieldLabel: '低溫商品',
                items: [
                    {
                        xtype: 'displayfield',
                        value: '運費'
                    },
                    {
                        xtype: 'textfield',
                        id: 'freight_low_money',
                        name: 'freight_low_money',
                        regex: /^([1-9]\d*|0)$/,
                        width: 40
                    },
                    {
                        xtype: 'displayfield',
                        value: '元，運費'
                    },
                    {
                        xtype: 'textfield',
                        id: 'freight_low_limit',
                        name: 'freight_low_limit',
                        regex: /^([1-9]\d*|0)$/,
                        width: 40
                    },
                    {
                        xtype: 'displayfield',
                        value: '元(含)以上免運費，逆物流運費'
                    },
                    {
                        xtype: 'textfield',
                        id: 'freight_return_low_money',
                        name: 'freight_return_low_money',
                        regex: /^([1-9]\d*|0)$/,
                        width: 40
                    },
                    {
                        xtype: 'displayfield',
                        value: '元'
                    }
                ]
            },
            {
                xtype: 'combobox',
                allowBlank: false,
                editable: false,
                fieldLabel: '管理人員',
                // hidden: true,//先隱藏掉,以免以后會再用
                id: 'manage_name',
                name: 'manage_name',
                store: pmStore,
                displayField: 'userName',
                valueField: 'userId',
                lastQuery: '',
                emptyText: "請選擇",
                // value: "0",
                width: 200,
                forceSelection: false
            },
            {
                xtype: 'fieldcontainer',
                layout: 'hbox',
                id: 'create_erp',
                width: 500,
                defaults: {
                    hideLabel: true
                },
                items: [
                    {
                        xtype: 'displayfield',
                        value: '廠商分類:',
                        width: 70
                    },
                    {
                        xtype: 'combo',
                        id: 'prod_cate',
                        name: 'prod_cate',
                        queryMode: 'local',
                        // value: "0",
                        emptyText: "請選擇商品類型",
                        displayField: 'parameterName',
                        valueField: 'ParameterCode',
                        store: ProductCateStore,
                        editable: false,
                        allowBlank: rowID ? true : false,
                        margins: '0 0 0 5',
                        width: 110,
                        listeners: {
                            "select": function (combo, record) {
                                var erp_id = Ext.getCmp("erp_id");
                                var procate = Ext.getCmp("prod_cate");
                                var buycate = Ext.getCmp("buy_cate");
                                var tax_type = Ext.getCmp("tax_type");
                                buycate.clearValue();
                                if (procate.getValue() != null) {
                                    buycate.setDisabled(false);
                                } else if (procate.getValue() == null) {
                                    buycate.setValue("");
                                    buycate.setDisabled(true);
                                }
                                BuyCateStore.removeAll();
                                comProd = true;
                                if (procate.getValue() !== 0 && buycate.getValue() !== 0 && buycate.getValue() !== null && tax_type.getValue() !== 0 && tax_type.getValue() !== null) {
                                    getErpId(procate.getValue(), buycate.getValue(), tax_type.getValue());
                                } else {
                                    Ext.getCmp("erp_id").setValue("");
                                }
                            }
                        }
                    },
                    {
                        xtype: 'combo',
                        name: 'buy_cate',
                        id: 'buy_cate',
                        lastQuery: '',
                        displayField: 'parameterName',
                        valueField: 'ParameterCode',
                        store: BuyCateStore,
                        //   value: 0,
                        emptyText: "請選擇購買類型",
                        disabled: true,
                        editable: false,
                        allowBlank: rowID ? true : false,
                        width: 120,
                        margins: '0 5 0 5',
                        listeners: {
                            beforequery: function (qe) {
                                if (comProd) {
                                    delete qe.combo.lastQuery;
                                    BuyCateStore.load({
                                        params: {
                                            topValue: Ext.getCmp("prod_cate").getValue()
                                        }
                                    });
                                    comProd = false;
                                }
                            },
                            "select": function (combo, record) {
                                var erp_id = Ext.getCmp("erp_id");
                                var prod_cate = Ext.getCmp("prod_cate");
                                var buy_cate = Ext.getCmp("buy_cate");
                                var tax_type = Ext.getCmp("tax_type");
                                if (prod_cate.getValue() !== 0 && buy_cate.getValue() !== 0 && buy_cate.getValue() !== null && tax_type.getValue() !== 0 && tax_type.getValue() !== null) {
                                    getErpId(prod_cate.getValue(), buy_cate.getValue(), tax_type.getValue());
                                } else {
                                    Ext.getCmp("erp_id").setValue("");
                                }
                            }
                        }
                    },
                    {
                        xtype: 'combo',
                        name: 'tax_type',
                        id: 'tax_type',
                        queryMode: 'local',
                        displayField: 'value',
                        valueField: 'id',
                        store: taxTypeStore,
                        typeAhead: true,
                        emptyText: "請選擇應稅免稅",
                        //value: 0,
                        editable: false,
                        forceSelection: false,
                        allowBlank: rowID ? true : false,
                        width: 120,
                        margins: '0 5 0 5',
                        listeners: {
                            "select": function (combo, record) {
                                var erp_id = Ext.getCmp("erp_id");
                                var prod_cate = Ext.getCmp("prod_cate");
                                var buy_cate = Ext.getCmp("buy_cate");
                                var tax_type = Ext.getCmp("tax_type");
                                if (prod_cate.getValue() !== 0 && buy_cate.getValue() !== 0 && buy_cate.getValue() !== null && tax_type.getValue() !== 0) {
                                    getErpId(prod_cate.getValue(), buy_cate.getValue(), tax_type.getValue());
                                } else {
                                    Ext.getCmp("erp_id").setValue("");
                                }
                            }
                        }
                    }
                ]
            },
            {
                xtype: 'textfield',
                fieldLabel: 'ERP 廠商編號',
                id: 'erp_id',
                //editable: false,
                allowBlank: false,
                readOnly: true,
                name: 'erp_id',
                labelWidth: 90
            },
            {
                xtype: 'fieldcontainer',
                combineErrors: true,
                layout: 'hbox',
                labelWidth: 90,
                items: [
                    {
                        xtype: 'textfield',
                        fieldLabel: '業績獎金門檻',
                        id: 'gigade_bunus_threshold',
                        name: 'gigade_bunus_threshold',
                        labelWidth: 90,
                        width: 200,
                        minValue: 0,
                        maxValue: 100,
                        regex: /^[-+]?([1-9]\d*|0)$/
                    },
                    {
                        xtype: 'textfield',
                        fieldLabel: '獎金百分比',
                        id: 'gigade_bunus_percent',
                        name: 'gigade_bunus_percent',
                        labelWidth: 90,
                        width: 210,
                        margins: '0 0 0 20',
                        minValue: 0,
                        maxValue: 100,
                        regex: /^[-+]?([1-9]\d*|0)$/
                    },
                    {
                        xtype: 'displayfield',
                        value: '百分比'
                    }
                ]
            }
        ]
    });
    var thirdForm = Ext.widget('form',
           {
               id: 'editFrm2',
               plain: true,
               frame: true,
               url: '/Vendor/SaveVendor',
               layout: 'anchor',
               items: [
               gdcontact,
               {
                   xtype: 'textareafield',
                   fieldLabel: '備註',
                   id: 'vendor_note',
                   name: 'vendor_note',
                   margin: '5 0 5 0',
                   labelWidth: 50,
                   width: 480
               }
               ],
               buttonAlign: 'right',
               buttons: [{
                   text: '保存',
                   id: 'save',
                   formBind: true,
                   disabled: true,
                   handler: function () {
                       var form = this.up('form').getForm();
                       var InsertValues = "";
                       if (form.isValid()) {
                           var gdcontact = Ext.getCmp("gdcontact").store.data.items;

                           for (var a = 0; a < gdcontact.length; a++) {
                               var type = gdcontact[a].get("contact_type");
                               var name = gdcontact[a].get("contact_name");
                               var phone1 = gdcontact[a].get("contact_phone1");
                               var phone2 = gdcontact[a].get("contact_phone2");
                               var mobile = gdcontact[a].get("contact_mobile");
                               var email = gdcontact[a].get("contact_email");

                               InsertValues += type + ',' + name + ',' + phone1 + ',' + phone2 + ',' + mobile + ',' + email + '|';
                           }
                           this.disable();
                           form.submit({
                               params: {
                                   vendor_id: VendorID,
                                   delconnect: delconnect,

                                   vendor_code: Ext.htmlEncode(Ext.getCmp("vendor_code").getValue()),
                                   vendor_status: Ext.htmlEncode(Ext.getCmp("vendor_status").getValue().vendor_status),
                                   vendor_email: Ext.htmlEncode(Ext.getCmp("vendor_email").getValue()),
                                   vendor_password: Ext.htmlEncode(Ext.getCmp("vendor_password").getValue()),
                                   vendor_name_full: Ext.htmlEncode(Ext.getCmp("vendor_name_full").getValue()),
                                   vendor_name_simple: Ext.htmlEncode(Ext.getCmp("vendor_name_simple").getValue()),
                                   vendor_invoice: Ext.htmlEncode(Ext.getCmp("vendor_invoice").getValue()),
                                   company_phone: Ext.htmlEncode(Ext.getCmp("company_phone").getValue()),
                                   company_fax: Ext.htmlEncode(Ext.getCmp("company_fax").getValue()),
                                   company_person: Ext.htmlEncode(Ext.getCmp("company_person").getValue()),
                                   company_zip: Ext.htmlEncode(Ext.getCmp('company_zip').getValue()),
                                   company_address: Ext.htmlEncode(Ext.getCmp('company_address').getValue()),
                                   invoice_zip: Ext.htmlEncode(Ext.getCmp('invoice_zip').getValue()),
                                   invoice_address: Ext.htmlEncode(Ext.getCmp('invoice_address').getValue()),
                                   prod_cate: Ext.htmlEncode(Ext.getCmp('prod_cate').getValue()),
                                   buy_cate: Ext.htmlEncode(Ext.getCmp('buy_cate').getValue()),
                                   tax_type: Ext.htmlEncode(Ext.getCmp('tax_type').getValue()),
                                   erp_id: Ext.htmlEncode(Ext.getCmp("erp_id").getValue()),
                                   conactValues: InsertValues,
                                   cost_percent: Ext.htmlEncode(Ext.getCmp("cost_percent").getValue()),
                                   creditcard_1_percent: Ext.htmlEncode(Ext.getCmp("creditcard_1_percent").getValue()),
                                   creditcard_3_percent: Ext.htmlEncode(Ext.getCmp("creditcard_3_percent").getValue()),
                                   agreement_createdate: Ext.htmlEncode(Ext.getCmp("agreement_createdate").getRawValue()),
                                   agreement_start: Ext.htmlEncode(Ext.getCmp("agreement_start").getRawValue()),
                                   agreement_end: Ext.htmlEncode(Ext.getCmp("agreement_end").getRawValue()),
                                   checkout_type: Ext.htmlEncode(Ext.getCmp("checkout_type").getValue().checkout_type),
                                   checkout_other: Ext.htmlEncode(Ext.getCmp("checkout_other").getValue()),
                                   bank_code: Ext.htmlEncode(Ext.getCmp("bank_code").getValue()),
                                   bank_name: Ext.htmlEncode(Ext.getCmp("bank_name").getValue()),
                                   bank_number: Ext.htmlEncode(Ext.getCmp("bank_number").getValue()),
                                   bank_account: Ext.htmlEncode(Ext.getCmp("bank_account").getValue()),
                                   freight_low_limit: Ext.htmlEncode(Ext.getCmp("freight_low_limit").getValue()),
                                   freight_low_money: Ext.htmlEncode(Ext.getCmp("freight_low_money").getValue()),
                                   freight_return_low_money: Ext.htmlEncode(Ext.getCmp("freight_return_low_money").getValue()),
                                   freight_normal_limit: Ext.htmlEncode(Ext.getCmp("freight_normal_limit").getValue()),
                                   freight_normal_money: Ext.htmlEncode(Ext.getCmp("freight_normal_money").getValue()),
                                   freight_return_normal_money: Ext.htmlEncode(Ext.getCmp("freight_return_normal_money").getValue()),
                                   // assist: Ext.htmlEncode(Ext.getCmp("assist").getValue().assist),
                                   //  dispatch: Ext.htmlEncode(Ext.getCmp("dispatch").getValue().dispatch),
                                   // product_mode: Ext.htmlEncode(Ext.getCmp("product_mode").getValue().product_mode),
                                   procurement_days: Ext.htmlEncode(Ext.getCmp("procurement_days").getValue()),
                                   self_send_days: Ext.htmlEncode(Ext.getCmp("self_send_days").getValue()),
                                   stuff_ware_days: Ext.htmlEncode(Ext.getCmp("stuff_ware_days").getValue()),
                                   dispatch_days: Ext.htmlEncode(Ext.getCmp("dispatch_days").getValue()),
                                   pm: Ext.htmlEncode(Ext.getCmp("manage_name").getValue()),
                                   gigade_bunus_percent: Ext.htmlEncode(Ext.getCmp("gigade_bunus_percent").getValue()),
                                   vendor_note: Ext.htmlEncode(Ext.getCmp("vendor_note").getValue()),
                                   gigade_bunus_threshold: Ext.htmlEncode(Ext.getCmp("gigade_bunus_threshold").getValue()),
                                   gigade_vendor_type: Ext.htmlEncode(Ext.getCmp("vendor_type").getValue()) //供應商類型
                               },
                               success: function (form, action) {
                                   var result = Ext.decode(action.response.responseText);
                                   if (result.success) {
                                       if (result.msg == 0) {
                                           Ext.Msg.alert(INFORMATION, "公司email不能重複，請修改您的輸入 ");
                                       } else {
                                           Ext.Msg.alert(INFORMATION, SUCCESS);
                                           VendorListStore.loadPage(1);
                                           editWin.close();
                                       }
                                   } else {
                                       Ext.Msg.alert(INFORMATION, FAILURE);
                                   }
                               },
                               failure: function (form, action) {
                                   Ext.Msg.alert(INFORMATION, FAILURE);
                               }
                           });
                       }
                   }
               }]
           });
    var allpan = new Ext.panel.Panel({
        width: 500,
        layout: 'card',
        border: 0,
        bodyStyle: 'padding:0px',
        defaults: {
            border: false
        },
        bbar: [
            {
                id: 'move-prev',
                text: '上一步',
                hidden: true,
                handler: function (btn) {
                    navigate(btn.up("panel"), "prev");
                },
                disabled: true
            },
            '->', // 一个长间隔, 使两个按钮分布在两边
            {
                id: 'move-next',
                text: '下一步',
                margins: '0 5 0 0',
                handler: function (btn) {
                    navigate(btn.up("panel"), "next");
                }
            }
        ],
        items: [firstForm, secondForm, thirdForm]
    });

    var editWin = Ext.create('Ext.window.Window', {
        title: '供應商編輯',
        iconCls: rowID ? "icon-user-edit" : "icon-user-add",
        id: 'editWin',
        width: 510,
        height: 480,
        y: 100,
        //height: document.documentElement.clientHeight * 260 / 783,
        layout: 'fit',
        items: [allpan],
        constrain: true, //束縛
        closeAction: 'destroy',
        modal: true,
        closable: false,
        tools: [
         {
             type: 'close',
             qtip: '是否關閉',
             handler: function (event, toolEl, panel) {
                 Ext.MessageBox.confirm(CONFIRM, IS_CLOSEFORM, function (btn) {
                     if (btn === "yes") {
                         Ext.getCmp('editWin').destroy();
                     }
                     else {
                         return false;
                     }
                 });
             }
         }],
        listeners: {
            'show': function () {
                if (row) {
                    //編輯時顯示供應商編號和供應商編碼
                    Ext.getCmp("vendor_id").show();
                    Ext.getCmp("vendor_code").show();
                    firstForm.getForm().loadRecord(row);
                    secondForm.getForm().loadRecord(row);
                    thirdForm.getForm().loadRecord(row);
                    initForm(row);
                }
                else {
                    //新增時不顯示編碼和編號
                    Ext.getCmp("vendor_id").hide();
                    Ext.getCmp("vendor_code").hide();
                    firstForm.getForm().reset();
                    secondForm.getForm().reset();
                    thirdForm.getForm().reset();
                }
            },
            scrollershow: function (scroller) {
                if (scroller && scroller.scrollEl) {
                    scroller.clearManagedListeners();
                    scroller.mon(scroller.scrollEl, 'scroll', scroller.onElScroll, scroller);
                }
            }
        }
    });

    if (rowID !== null) {
        var row = null;
        edit_VendorListStore.load({
            params: { relation_id: rowID },
            callback: function () {
                row = edit_VendorListStore.getAt(0);
                VendorID = rowID;
                editWin.show();
            }
        });

    }
    else {
        editWin.show();
    }
    function addTr() {
        if (ContactStore.getCount() < 5) {
            if (ContactStore.getCount() === 0) {
                ContactStore.add({
                    contact_type: '出貨聯絡窗口',
                    contact_name: '',
                    contact_phone1: '',
                    contact_phone2: '',
                    contact_mobile: '',
                    contact_email: ''
                });
                cellEditing.startEditByPosition({ row: 0, column: 2 });
            } else {
                ContactStore.add({
                    contact_type: '請選擇',
                    contact_name: '',
                    contact_phone1: '',
                    contact_phone2: '',
                    contact_mobile: '',
                    contact_email: ''
                });
                if (ContactStore.getCount() > 1) {
                    cellEditing.startEditByPosition({ row: ContactStore.getCount() - 1, column: 1 });
                }
            }
        } else {
            Ext.Msg.alert(INFORMATION, "最多只有五個聯繫人");
        }
    }


    function initForm(row) {
        //if (isSave == false) {
        //    Ext.getCmp("save").hide();
        //    //firstForm.items.each(function (item, index, length) {
        //    //    item.readOnly = true;
        //    //});
        //}
        Ext.getCmp('editcon').show();
        Ext.getCmp("vendor_password").setValue("");
        //Ext.getCmp('vendorsec').hide();

        switch (row.data.vendor_status) {
            case 1:
                Ext.getCmp("active").setValue(true);
                break;
            case 2:
                Ext.getCmp("nonactive").setValue(true);
                Ext.getCmp("thnactive").show();
                break;
            case 3:
                Ext.getCmp("thnactive").setValue(true).show();
                Ext.getCmp("vendor_status").setDisabled(true)
                break;
        }
        if (row.data.product_manage == 0) {
            Ext.getCmp("manage_name").setValue("0");
        }

        //修改時對供應商類型賦值
        //if (row.data.vendor_type != "" && row.data.vendor_type != null) {
        //    var siteIDs = row.data.vendor_type.toString().split(',');
        //    var combobox = Ext.getCmp('vendor_type_name');
        //    var store = combobox.store;
        //    var arrTemp = new Array();
        //    for (var i = 0; i < siteIDs.length; i++) {
        //        arrTemp.push(store.getAt(store.find("ParameterCode", siteIDs[i].toString())));
        //    }
        //    combobox.setValue(arrTemp);
        //}


        AreaStore.load({
            callback: function () {
                Ext.getCmp("cob_ccity_a").setValue(row.data.c_bigcode);
                CityStore.load({
                    params: {
                        topValue: row.data.c_bigcode
                    },
                    callback: function () {
                        Ext.getCmp("cob_ccity_c").setValue(row.data.c_midcode);
                        ZipStore.load({
                            params: {
                                topValue: row.data.c_midcode,
                                topText: row.data.c_middle
                            },
                            callback: function () {
                                Ext.getCmp("company_zip").setValue(row.data.c_zipcode);
                            }
                        });

                    }
                });
            }
        });

        InvoiceAreaStore.load({
            callback: function () {
                Ext.getCmp("cob_ccity_ai").setValue(row.data.i_bigcode);
                InvoiceCityStore.load({
                    params: {
                        topValue: row.data.i_bigcode
                    },
                    callback: function () {
                        Ext.getCmp("cob_ccity_i").setValue(row.data.i_midcode);
                        InvoiceZipStore.load({
                            params: {
                                topValue: row.data.i_midcode,
                                topText: row.data.i_middle
                            },
                            callback: function () {
                                Ext.getCmp("invoice_zip").setValue(row.data.i_zipcode);
                            }
                        });
                    }
                });
            }
        });
        Ext.getCmp("create_erp").hide();

        //secondPanel
        if (row.data.agr_date !== "undefined") {
            Ext.getCmp("agreement_createdate").setValue(row.data.agr_date.substring(0, 10));
        }
        if (row.data.agr_start !== "undefined") {
            Ext.getCmp("agreement_start").setValue(row.data.agr_start.substring(0, 10));
        }
        if (row.data.agr_end !== "undefined") {
            Ext.getCmp("agreement_end").setValue(row.data.agr_end.substring(0, 10));
        }

        switch (row.data.checkout_type) {
            case "1":
                Ext.getCmp("1").setValue(true);
                Ext.getCmp("checkout_other").setDisabled(true);
                break;
            case "2":
                Ext.getCmp("2").setValue(true);
                Ext.getCmp("checkout_other").setDisabled(true);
                break;
            case "3":
                Ext.getCmp("3").setValue(true);
                Ext.getCmp("checkout_other").setDisabled(false);
                break;

        }
        if (row.data.checkout_other !== "") {
            Ext.getCmp("checkout_other").setValue(row.data.checkout_other);
        }

        //thirdPanel

        Ext.getCmp("vendor_note").setValue(row.data.vendor_note);

        if (row.data.contact_type_1 !== 0) {
            ContactStore.removeAll();
            ContactStore.add({
                contact_type: '出貨聯絡窗口',
                contact_name: row.data.contact_name_1,
                contact_phone1: row.data.contact_phone_1_1,
                contact_phone2: row.data.contact_phone_2_1,
                contact_mobile: row.data.contact_mobile_1,
                contact_email: row.data.contact_email_1
            });

        }
        if (row.data.contact_type_2 !== 0) {
            ContactStore.add({
                contact_type: connecttypeStore.data.items[row.data.contact_type_2].get("value"),
                contact_name: row.data.contact_name_2,
                contact_phone1: row.data.contact_phone_1_2,
                contact_phone2: row.data.contact_phone_2_2,
                contact_mobile: row.data.contact_mobile_2,
                contact_email: row.data.contact_email_2
            });

        }
        if (row.data.contact_type_3 !== 0) {
            ContactStore.add({
                contact_type: connecttypeStore.data.items[row.data.contact_type_3].get("value"),
                contact_name: row.data.contact_name_3,
                contact_phone1: row.data.contact_phone_1_3,
                contact_phone2: row.data.contact_phone_2_3,
                contact_mobile: row.data.contact_mobile_3,
                contact_email: row.data.contact_email_3
            });

        }
        if (row.data.contact_type_4 !== 0) {
            ContactStore.add({
                contact_type: connecttypeStore.data.items[row.data.contact_type_4].get("value"),
                contact_name: row.data.contact_name_4,
                contact_phone1: row.data.contact_phone_1_4,
                contact_phone2: row.data.contact_phone_2_4,
                contact_mobile: row.data.contact_mobile_4,
                contact_email: row.data.contact_email_4
            });

        }
        if (row.data.contact_type_5 !== 0) {
            ContactStore.add({
                contact_type: connecttypeStore.data.items[row.data.contact_type_5].get("value"),
                contact_name: row.data.contact_name_5,
                contact_phone1: row.data.contact_phone_1_5,
                contact_phone2: row.data.contact_phone_2_5,
                contact_mobile: row.data.contact_mobile_5,
                contact_email: row.data.contact_email_5
            });

        }
    }

}

function getErpId(prod_cate, buy_cate, tax_type) {
    Ext.Ajax.request({
        url: '/Vendor/GetErpId',
        method: 'post',
        params: {
            prod_cate: prod_cate,
            buy_cate: buy_cate,
            tax_type: tax_type
        },
        success: function (msg) {
            var resMsg = eval("(" + msg.responseText + ")");
            if (resMsg.success) {
                Ext.getCmp("erp_id").setValue(resMsg.data);
            } else {
                Ext.Msg.alert(INFORMATION, FAILURE);
            }
        },
        failure: function () {
            Ext.Msg.alert(INFORMATION, FAILURE);
        }
    });
}

