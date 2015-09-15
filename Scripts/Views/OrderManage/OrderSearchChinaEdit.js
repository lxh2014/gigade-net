function editFunction(RowID, VendorListStore) {
    var VendorID = null;
    var currentPanel = 0;
    if (RowID != null) {
        VendorID = RowID.data.vendor_id;
    }
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

    ZipStore.on('beforeload', function () {
        Ext.apply(ZipStore.proxy.extraParams, {
            topValue: Ext.getCmp("cob_ccity_c").getValue(),
            topText: Ext.getCmp("cob_ccity_c").rawValue
        });
    });

    InvoiceZipStore.on('beforeload', function () {
        Ext.apply(InvoiceZipStore.proxy.extraParams, {
            topValue: Ext.getCmp("cob_ccity_i").getValue(),
            topText: Ext.getCmp("cob_ccity_i").rawValue
        });
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
        autoLoad: true,
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
            { id: '0', value: '請選擇應稅免稅' },
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
                if (e.record.data.contact_type == "出貨聯絡窗口") {
                    if (e.rowIdx == 0 && e.colIdx == 1) {
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
                                if (btn == 'yes') {
                                    if (ContactStore.getCount() > 1) {
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
                header: '窗口屬性', dataIndex: 'contact_type', width: 90, align: 'left',
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
                    queryMode: 'local'
                }
            },
            { header: '姓名', dataIndex: 'contact_name', width: 65, align: 'center', editor: { id: 'txt_contactname' } },
            {
                header: '電話1', dataIndex: 'contact_phone1', width: 70, align: 'center', editor: { id: 'txt_contactphone' },
                editor: {
                    xtype: "textfield",
                    allowBlank: false,
                    regex: /^([0-9]\d*)$/
                }
            },
            {
                header: '電話2', dataIndex: 'contact_phone2', width: 70, align: 'center', editor: { id: 'txt_contactphone1' },
                editor: {
                    xtype: "textfield",
                    minLength: 5,
                    maxLength: 10,
                    regex: /^([0-9]\d*)$/
                }
            },
            {
                header: '手機', dataIndex: 'contact_mobile', width: 70, align: 'center', editor: { id: 'txt_contactmobile' },
                editor: {
                    xtype: "textfield",
                    allowBlank: false,
                    minLength: 10,
                    maxLength: 10,
                    regex: /^([0-9]\d*)$/
                }
            },
            {
                header: 'E-mail', dataIndex: 'contact_email', width: 80, align: 'center', editor: { id: 'txt_contactemail' },
                editor: {
                    xtype: "textfield",
                    allowBlank: false,
                    vtype: 'email'
                }
            }
        ],
        tbar: [{ xtype: 'button', text: '新增聯繫人', iconcls: 'icon-user-add', handler: addTr }],
        listeners: {
            beforerender: function () {
                if (!RowID) {
                    ContactStore.load({
                        callback: function (records, operation, success) {
                            if (records.length <= 0) {
                                addTr();
                            }
                        }
                    });
                }
            }
        }
    });

    var navigate = function (panel, direction) {
        var layout = panel.getLayout();
        if ('next' == direction) {
            if (currentPanel == 0) {
                var formf = firstForm.getForm();
                if (!RowID) {
                    if (formf.isValid()) {
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
                } else {
                    if (formf.isValid()) {
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
                var forms = secondForm.getForm();
                if (forms.isValid()) {
                    Ext.getCmp('move-prev').setDisabled(false);
                    Ext.getCmp('move-prev').show();
                    layout[direction]();
                    currentPanel++;
                    Ext.getCmp('move-next').hide();
                }
            }
        } else {
            Ext.getCmp('move-prev').setDisabled(false);
            layout[direction]();
            currentPanel--;
            if (currentPanel == 0) {
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
        items: [
            {
                xtype: 'fieldcontainer',
                combineErrors: true,
                id: 'editcon',
                layout: 'hbox',
                //hidden: true,
                defaults: {
                    width: 210,
                    labelWidth: 70
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
                    labelWidth: 70
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
                    labelWidth: 70
                },
                items: [
                    {
                        fieldLabel: '統一編號',
                        xtype: 'textfield',
                        id: 'vendor_invoice',
                        name: 'vendor_invoice',
                        allowBlank: false,
                        regex: /^[-+]?([1-9]\d*|0)$/
                    },
                    {
                        fieldLabel: '公司Email',
                        xtype: 'textfield',
                        id: 'vendor_email',
                        margin: '0 8 0 20',
                        name: 'vendor_email',
                        allowBlank: false,
                        vtype: 'email'
                    }
                ]
            },
            {
                xtype: 'fieldcontainer',
                combineErrors: true,
                id: 'vendorsec',
                layout: 'hbox',
                defaults: {
                    width: 210,
                    labelWidth: 70
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
                        columns: 2,
                        vertical: true,
                        items: [
                            { boxLabel: '啟用', id: 'active', inputValue: '1', checked: true },
                            { boxLabel: '停用', id: 'nonactive', inputValue: '0' }
                        ]
                    },
                    {
                        fieldLabel: '密碼',
                        xtype: 'textfield',
                        id: 'vendor_password',
                        name: 'vendor_password',
                        // hidden: true,
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
                    labelWidth: 70
                },
                items: [
                    {
                        fieldLabel: '公司電話',
                        xtype: 'textfield',
                        id: 'company_phone',
                        name: 'company_phone',
                        allowBlank: false,
                        regex: /^(00886-?)?(\d{2,4})-?(\d{3})-?(\d{4})(-?\d{3,4})?$/
                    },
                    {
                        fieldLabel: '公司傳真',
                        xtype: 'textfield',
                        margin: '0 8 0 20',
                        id: 'company_fax',
                        name: 'company_fax',
                        allowBlank: false,
                        regex: /^(00886-?)?(\d{2,4})-?(\d{3})-?(\d{4})(-?\d{3,4})?$/
                    }
                ]
            },
            {
                fieldLabel: '公司負責人',
                xtype: 'textfield',
                id: 'company_person',
                name: 'company_person',
                allowBlank: false,
                width: 210,
                labelWidth: 70
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
                        width: 70
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
                        queryMode: 'remote',
                        allowBlank: true,
                        margins: '0 0 0 5',
                        width: 80,
                        listeners: {
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
                        store: ZipStore,
                        editable: false,
                        queryMode: 'remote',
                        allowBlank: true,
                        width: 100,
                        margins: '0 5 0 5',
                        listeners: {
                            beforequery: function (qe) {
                                if (comCity) {
                                    delete qe.combo.lastQuery;
                                    ZipStore.load({
                                        params: {
                                            topValue: Ext.getCmp("cob_ccity_c").getValue()
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
                        width: 180,
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
                        xtype: 'displayfield',
                        value: '發票地址:',
                        //id: 'invoice_address',
                        width: 70
                    },
                    {
                        xtype: 'combo',
                        id: 'cob_ccity_i',
                        queryMode: 'local',
                        displayField: 'middle',
                        valueField: 'middlecode',
                        store: InvoiceCityStore,
                        editable: false,
                        queryMode: 'remote',
                        allowBlank: true,
                        margins: '0 0 0 5',
                        width: 80,
                        listeners: {
                            "select": function (combo, record) {
                                var z = Ext.getCmp("invoice_zip");
                                z.clearValue();
                                InvoiceZipStore.removeAll();
                                comCity = true;
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
                        store: InvoiceZipStore,
                        editable: false,
                        queryMode: 'remote',
                        allowBlank: true,
                        width: 100,
                        margins: '0 5 0 5',
                        listeners: {
                            beforequery: function (qe) {
                                if (comCity) {
                                    delete qe.combo.lastQuery;
                                    InvoiceZipStore.load({
                                        params: {
                                            topValue: Ext.getCmp("cob_ccity_i").getValue()
                                        }
                                    });
                                    comCity = false;
                                }
                            }
                        }
                    },
                    {
                        id: 'invoice_address',
                        xtype: 'textfield',
                        width: 180,
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
                        labelWidth: 70,
                        fieldLabel: '成本',
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
                xtype: 'radiogroup',
                hidden: false,
                id: 'assist',
                name: 'assist',
                fieldLabel: '出貨模式',
                colName: 'assist',
                defaults: {
                    name: 'assist',
                    margin: '5 8 0 0'
                },
                width: 400,
                lableWidth: 70,
                columns: 2,
                vertical: true,
                items: [
                    { boxLabel: '廠商自行管理', id: 'cs_self', inputValue: '0', checked: true },
                    { boxLabel: '協助轉單', id: 'help', inputValue: '1' }
                ]
            },
            {
                xtype: 'radiogroup',
                hidden: false,
                id: 'dispatch',
                name: 'dispatch',
                width: 400,
                lableWidth: 70,
                fieldLabel: '調度倉模式',
                colName: 'dispatch',
                defaults: {
                    name: 'dispatch',
                    margin: '5 8 0 0'
                },
                columns: 2,
                vertical: true,
                items: [
                    { boxLabel: '自行出貨', id: 'self', inputValue: '0', checked: true },
                    { boxLabel: '進調度倉', id: 'diaoducang', inputValue: '1' }
                ]
            },
            {
                xtype: 'radiogroup',
                hidden: false,
                id: 'product_mode',
                name: 'product_mode',
                fieldLabel: '寄倉模式',
                colName: 'product_mode',
                defaults: {
                    name: 'product_mode',
                    margin: '  8 0 0'
                },
                width: 400,
                lableWidth: 70,
                columns: 2,
                vertical: true,
                items: [
                    { boxLabel: '非寄倉', id: 'no', inputValue: '1', checked: true },
                    { boxLabel: '寄倉', id: 'yes', inputValue: '2' }
                ]
            },
            {
                xtype: 'displayfield',
                value: '  此欄位只提供會計備註用，不會異動到商品寄倉選項'
            }
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
                         fieldLabel: '信用卡一期手續費',
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
                items: [
                     {
                         xtype: 'textfield',
                         name: 'creditcard_3_percent',
                         id: 'creditcard_3_percent',
                         labelWidth: 120,
                         fieldLabel: '信用卡三期手續費',
                         minValue: 0,
                         maxValue: 10,
                         regex: /^(([0-9]+\.[0-9]*[1-9][0-9]*)|([0-9]*[1-9][0-9]*\.[0-9]+)|([0-9]*[1-9][0-9]*))$/
                     },
                     {
                         xtype: 'displayfield',
                         value: '百分比'
                     }
                ]
            },
            {
                xtype: "datetimefield",
                id: 'agreement_createdate',
                name: 'agreement_createdate',
                format: 'Y-m-d H:i:s',
                labelWidth: 120,
                fieldLabel: '合約簽定日期',
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
                          xtype: "datetimefield",
                          id: 'agreement_start',
                          name: 'agreement_start',
                          format: 'Y-m-d H:i:s',
                          allowBlank: false,
                          editable: false,
                          submitValue: true,
                          value: Tomorrow(),
                          listeners: {
                              select: function (a, b, c) {
                                  var start = Ext.getCmp("agreement_start");
                                  var end = Ext.getCmp("agreement_end");
                                  var s_date = new Date(start.getValue());
                                  end.setValue(new Date(s_date.setMonth(s_date.getMonth() + 1)));

                              }
                          }

                      },
                      {
                          xtype: 'displayfield',
                          value: '~'
                      },
                        {
                            xtype: "datetimefield",
                            format: 'Y-m-d H:i:s',
                            id: 'agreement_end',
                            editable: false,
                            name: 'agreement_end',
                            allowBlank: false,
                            submitValue: true,
                            value: new Date(Tomorrow().setMonth(Tomorrow().getMonth() + 1)),
                            listeners: {
                                select: function (a, b, c) {
                                    var start = Ext.getCmp("agreement_start");
                                    var end = Ext.getCmp("agreement_end");
                                    if (end.getValue() < start.getValue()) {
                                        Ext.Msg.alert(INFORMATION, '開始時間不能大於結束時間');
                                        end.setValue(new Date(Tomorrow().setMonth(Tomorrow().getMonth() + 1)));
                                    }
                                }
                            }
                        }
                ]
            },
             {
                 xtype: 'fieldcontainer',
                 combineErrors: true,
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

                                             Ext.getCmp('checkout_other').allowBlank = true;
                                             Ext.getCmp('checkout_other').setDisabled(true);

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
                             allowBlank: false,
                             regex: /^[-+]?([1-9]\d*|0)$/
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
                             allowBlank: false,
                             regex: /^[-+]?([1-9]\d*|0)$/
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
                 allowBlank: true,
                 editable: false,
                 fieldLabel: '管理人員',
                 hidden: false,
                 id: 'manage_name',
                 name: 'manage_name',
                 store: pmStore,
                 displayField: 'userName',
                 valueField: 'userId',
                 typeAhead: true,
                 value: 0,
                 width: 200,
                 forceSelection: false,
                 emptyText: '請選擇'
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
         value: "0",
         displayField: 'parameterName',
         valueField: 'ParameterCode',
         store: ProductCateStore,
         editable: false,
         allowBlank: true,
         margins: '0 0 0 5',
         width: 80,
         listeners: {
             "select": function (combo, record) {
                 var z = Ext.getCmp("buy_cate");
                 //z.clearValue();
                 z.setValue("0");
                 BuyCateStore.removeAll();
                 BuyCateStore.load({
                     params: {
                         topValue: Ext.getCmp("prod_cate").getValue()
                     }
                 });
                 comProd = true;
             }
         }
     },
     {
         xtype: 'combo',
         name: 'buy_cate',
         id: 'buy_cate',
         queryMode: 'local',
         displayField: 'parameterName',
         valueField: 'ParameterCode',
         store: BuyCateStore,
         value: "0",
         editable: false,
         allowBlank: true,
         width: 120,
         margins: '0 5 0 5',
         listeners: {
             //beforequery: function (qe) {
             //    if (comProd) {
             //        //delete qe.combo.lastQuery;
             //        //BuyCateStore.load({
             //        //    params: {
             //        //        topValue: Ext.getCmp("prod_cate").getValue()
             //        //    }
             //        //});
             //        Ext.getCmp("buy_cate").setValue("0");
             //        comProd = false;
             //    }
             //},
             "select": function (combo, record) {
                 var erp_id = Ext.getCmp("erp_id");
                 var prod_cate = Ext.getCmp("prod_cate");
                 var buy_cate = Ext.getCmp("buy_cate");
                 var tax_type = Ext.getCmp("tax_type");
                 if (prod_cate.getValue() != "0" && buy_cate.getValue() != "0" && tax_type.getValue() != "0") {
                     getErpId(prod_cate.getValue(), buy_cate.getValue(), tax_type.getValue())
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
         value: 0,
         forceSelection: false,
         allowBlank: true,
         width: 120,
         margins: '0 5 0 5',
         listeners: {
             "select": function (combo, record) {
                 var erp_id = Ext.getCmp("erp_id");
                 var prod_cate = Ext.getCmp("prod_cate");
                 var buy_cate = Ext.getCmp("buy_cate");
                 var tax_type = Ext.getCmp("tax_type");
                 if (prod_cate.getValue() != "0" && buy_cate.getValue() != "0" && tax_type.getValue() != "0") {
                     getErpId(prod_cate.getValue(), buy_cate.getValue(), tax_type.getValue())
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
    var thirdForm = Ext.widget('form', {
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

                        InsertValues += type + ',' + name + ',' + phone1 + ',' + phone2 + ',' + mobile + ',' + email + ';'
                    }
                    alert("pm:" + Ext.getCmp("manage_name").getValue());
                    form.submit({
                        params: {
                            vendor_id: VendorID,
                            vendor_code: Ext.htmlEncode(Ext.getCmp("vendor_code").getValue()),
                            vendor_status: Ext.htmlEncode(Ext.getCmp("vendor_status").getValue().vendor_status),
                            vendor_email: Ext.htmlEncode(Ext.getCmp("vendor_email").getValue()),
                            vendor_password: hex_sha256(Ext.htmlEncode(Ext.getCmp("vendor_password").getValue())),
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
                            assist: Ext.htmlEncode(Ext.getCmp("assist").getValue().assist),
                            dispatch: Ext.htmlEncode(Ext.getCmp("dispatch").getValue().dispatch),
                            product_mode: Ext.htmlEncode(Ext.getCmp("product_mode").getValue().product_mode),
                            pm: Ext.htmlEncode(Ext.getCmp("manage_name").getValue()),
                            gigade_bunus_percent: Ext.htmlEncode(Ext.getCmp("gigade_bunus_percent").getValue()),
                            vendor_note: Ext.htmlEncode(Ext.getCmp("vendor_note").getValue()),
                            gigade_bunus_threshold: Ext.htmlEncode(Ext.getCmp("gigade_bunus_threshold").getValue())
                        },
                        success: function (form, action) {
                            var result = Ext.decode(action.response.responseText);
                            if (result.success) {
                                Ext.Msg.alert(INFORMATION, SUCCESS);
                                VendorListStore.loadPage(1);
                                editWin.close();
                            } else {
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
        title: '供應商管理',
        iconCls: RowID ? "icon-user-edit" : "icon-user-add",
        id: 'editWin',
        width: 510,
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
                        if (btn == "yes") {
                            Ext.getCmp('editWin').destroy();
                        } else {
                            return false;
                        }
                    });
                }
            }
        ],
        listeners: {
            'show': function () {
                if (RowID) {
                    //編輯時顯示供應商編號和供應商編碼
                    Ext.getCmp("vendor_id").show();
                    Ext.getCmp("vendor_code").show();
                    firstForm.getForm().loadRecord(RowID);
                    secondForm.getForm().loadRecord(RowID);
                    thirdForm.getForm().loadRecord(RowID);
                    initForm(RowID);
                }
                else {
                    //新增時不顯示編碼和編號
                    Ext.getCmp("vendor_id").hide();
                    Ext.getCmp("vendor_code").hide();
                    firstForm.getForm().reset();
                    secondForm.getForm().reset();
                    thirdForm.getForm().reset();
                }
            }
        }
    });
    editWin.show();

    function addTr() {
        if (ContactStore.getCount() <= 5) {
            if (ContactStore.getCount() == 0) {
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
            Ext.Msg.alert("最多只有五個聯絡人");
        }
    }

    function Tomorrow() {
        var d;
        var s = "";
        d = new Date();                             // 创建 Date 对象。
        s += d.getFullYear() + "/";                     // 获取年份。
        s += (d.getMonth() + 1) + "/";              // 获取月份。
        s += d.getDate() + 1;                          // 获取日。
        return (new Date(s));                                 // 返回日期。
    }
    function initForm(row) {
        Ext.getCmp('editcon').show();
        Ext.getCmp("vendor_password").setValue("");
        //Ext.getCmp('vendorsec').hide();
        switch (row.data.vendor_status) {
            case 0:
                Ext.getCmp("nonactive").setValue(true);
                break;
            case 1:
                Ext.getCmp("active").setValue(true);
                break;
        };
        switch (row.data.assist) {
            case 0:
                Ext.getCmp("cs_self").setValue(true);
                break;
            case 1:
                Ext.getCmp("help").setValue(true);
                break;
        };
        switch (row.data.dispatch) {
            case 0:
                Ext.getCmp("self").setValue(true);
                break;
            case 1:
                Ext.getCmp("diaoducang").setValue(true);
                break;
        };
        switch (row.data.product_mode) {
            case 1:
                Ext.getCmp("no").setValue(true);
                break;
            case 2:
                Ext.getCmp("yes").setValue(true);
                break;
        };
        Ext.getCmp("cob_ccity_c").setValue(row.data.c_middle);
        Ext.getCmp("company_zip").setValue(row.data.c_zip);
        Ext.getCmp("cob_ccity_i").setValue(row.data.i_middle);
        Ext.getCmp("invoice_zip").setValue(row.data.i_zip);
        Ext.getCmp("create_erp").hide();

        //secondPanel
        if (row.data.agreement_createdate != 0) {
            Ext.getCmp("agreement_createdate").setValue(row.data.agr_date);
        }
        if (row.data.agreement_start != 0) {
            Ext.getCmp("agreement_start").setValue(row.data.agr_start);
        }
        if (row.data.agreement_end != 0) {
            Ext.getCmp("agreement_end").setValue(row.data.agr_end);
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
        if (row.data.checkout_other != "") {
            Ext.getCmp("checkout_other").setValue(row.data.checkout_other);
        }

        //thirdPanel
        Ext.getCmp("vendor_note").setValue(row.data.vendor_note);

        if (row.data.contact_type_1 != 0) {
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
        if (row.data.contact_type_2 != 0) {
            ContactStore.add({
                contact_type: connecttypeStore.data.items[row.data.contact_type_2].get("value"),
                contact_name: row.data.contact_name_2,
                contact_phone1: row.data.contact_phone_1_2,
                contact_phone2: row.data.contact_phone_2_2,
                contact_mobile: row.data.contact_mobile_2,
                contact_email: row.data.contact_email_2
            });
        }
        if (row.data.contact_type_3 != 0) {
            ContactStore.add({
                contact_type: connecttypeStore.data.items[row.data.contact_type_3].get("value"),
                contact_name: row.data.contact_name_3,
                contact_phone1: row.data.contact_phone_1_3,
                contact_phone2: row.data.contact_phone_2_3,
                contact_mobile: row.data.contact_mobile_3,
                contact_email: row.data.contact_email_3
            });
        }
        if (row.data.contact_type_4 != 0) {
            ContactStore.add({
                contact_type: connecttypeStore.data.items[row.data.contact_type_4].get("value"),
                contact_name: row.data.contact_name_4,
                contact_phone1: row.data.contact_phone_1_4,
                contact_phone2: row.data.contact_phone_2_4,
                contact_mobile: row.data.contact_mobile_4,
                contact_email: row.data.contact_email_4
            });
        }
        if (row.data.contact_type_5 != 0) {
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