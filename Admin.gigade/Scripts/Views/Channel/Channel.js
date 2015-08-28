var comCity;
var InvoiceComCity;
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


//公司地址Store
var CityStore = Ext.create('Ext.data.Store', {
    autoDestroy: true,
    model: 'gigade.City',
    remoteSort: false,
    autoLoad: false,
    proxy: {
        type: 'ajax',
        url: "/Channel/QueryCity",
        reader: {
            type: 'json',
            root: 'items'
        }
    }
});

//公司郵編Store
var ZipStore = Ext.create('Ext.data.Store', {
    autoDestroy: true,
    remoteSort: false,
    autoLoad: false,
    model: 'gigade.Zip',
    proxy: {
        type: 'ajax',
        url: "/Channel/QueryZip",
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
        url: "/Channel/QueryCity",
        reader: {
            type: 'json',
            root: 'items'
        }
    }
});

//發票郵編Store
var InvoiceZipStore = Ext.create('Ext.data.Store', {
    autoDestroy: true,
    remoteSort: false,
    autoLoad: false,
    model: 'gigade.Zip',
    proxy: {
        type: 'ajax',
        url: "/Channel/QueryZip",
        actionMethods: 'post',
        reader: {
            type: 'json',
            root: 'items'
        }
    }
});

var ModelInStore = Ext.create('Ext.data.Store', {
    fields: ['text', 'value'],
    data: [
        { "text": ModelIn_ONE, "value": "1" },
        { "text": ModelIn_TWO, "value": "2" },
        { "text": ModelIn_THREE, "value": "3" }//add by jiajun 2014/08/14
    ]
});

ZipStore.on('beforeload', function () {
    Ext.apply(ZipStore.proxy.extraParams,
    {
        topValue: Ext.getCmp("cob_ccity").getValue(),
        topText: Ext.getCmp("cob_ccity").rawValue
    });
});

InvoiceZipStore.on('beforeload', function () {
    Ext.apply(InvoiceZipStore.proxy.extraParams,
    {
        topValue: Ext.getCmp("cob_cominvoicecity").getValue(),
        topText: Ext.getCmp("cob_cominvoicecity").rawValue
    });
});

Ext.onReady(function () {
    $("#main").height(document.documentElement.clientHeight - 5);

    Ext.create('Ext.form.Panel', {
        renderTo: 'txt_fullname', border: false,
        items: { id: 'txt_fname', xtype: 'textfield', width: 200, name: 'fname', allowBlank: false }
    });

    Ext.create('Ext.form.Panel', {
        renderTo: 'txt_simplename', border: false,
        items: { id: 'txt_sname', xtype: 'textfield', width: 200, name: 'sname', allowBlank: false }
    });

    Ext.create('Ext.form.Panel', {
        renderTo: 'txt_invoice', border: false,
        items: { id: 'txt_cominvoice', xtype: 'textfield', width: 200, name: 'cominvoice', allowBlank: false }
    });

    Ext.create('Ext.form.Panel', {
        renderTo: 'txt_email', border: false,
        items: { id: 'txt_comemail', xtype: 'textfield', width: 200, name: 'comemail' }
    });

    Ext.create('Ext.form.Panel', {
        renderTo: 'txt_comphone', border: false, width: 300,
        items: [
        {
            xtype: 'fieldcontainer', fieldLabel: '', msgTarget: 'under', layout: 'hbox',
            defaults: {
                hideLabel: true
            },
            items: [
                { id: 'txt_cphone', xtype: 'textfield', width: 40, name: 'cphone' },
                { xtype: 'displayfield', value: '—', margins: '0 5 0 5' },
                { id: 'txt_cphonenum', xtype: 'textfield', width: 140, name: 'cphonenum' }
            ]
        }]
    });

    Ext.create('Ext.form.Panel', {
        renderTo: 'txt_comfax', border: false, width: 300,
        items: [
        {
            xtype: 'fieldcontainer', fieldLabel: '', msgTarget: 'under', layout: 'hbox',
            defaults: {
                hideLabel: true
            },
            items: [
                { id: 'txt_cfax', xtype: 'textfield', width: 40, name: 'cfax' },
                { xtype: 'displayfield', value: '—', margins: '0 5 0 5' },
                { id: 'txt_cfaxnum', xtype: 'textfield', width: 140, name: 'cfaxnum' }
            ]
        }]
    });

    Ext.create('Ext.form.Panel', {
        renderTo: 'cob_comcity', border: false, width: 500,
        items: [{
            xtype: 'fieldcontainer', msgTarget: 'under', layout: 'hbox',
            defaults: {
                hideLabel: true
            }, items: [{
                xtype: 'combo', id: 'cob_ccity', queryMode: 'local', displayField: 'middle', valueField: 'middlecode', store: CityStore, editable: false, queryMode: 'remote', allowBlank: true, width: 80,
                listeners: {
                    "select": function (combo, record) {
                        var z = Ext.getCmp("cob_czip");
                        z.clearValue();
                        ZipStore.removeAll();
                        comCity = true;
                    }
                }
            }, {
                xtype: 'combo', name: 'czip', id: 'cob_czip', queryMode: 'local', displayField: 'small', valueField: 'zipcode', store: ZipStore, editable: false, queryMode: 'remote', allowBlank: true, width: 100, margins: '0 5 0 5',
                listeners: {
                    beforequery: function (qe) {
                        if (comCity) {
                            delete qe.combo.lastQuery;
                            ZipStore.load({
                                params: {
                                    topValue: Ext.getCmp("cob_ccity").getValue()
                                }
                            });
                            comCity = false;
                        }
                    },
                    "select": function (combo, record) {
                        var ArrZip = Ext.getCmp("cob_czip").getRawValue().split('/');
                        Ext.getCmp('txt_caddress').setValue(Ext.getCmp("cob_ccity").getRawValue() + ArrZip[1]);
                    }
                }
            }, { id: 'txt_caddress', xtype: 'textfield', width: 300, name: 'caddress' }
            ]
        }]
    });

    Ext.create('Ext.form.Panel', {
        renderTo: 'txt_invoicetitle', border: false,
        items: { id: 'txt_cominvoicetitle', xtype: 'textfield', width: 200, name: 'cominvoicetitle', allowBlank: false }
    });

    Ext.create('Ext.form.Panel', {
        renderTo: 'cob_invoicecity', border: false, width: 500,
        items: [{
            xtype: 'fieldcontainer', msgTarget: 'under', layout: 'hbox',
            defaults: {
                hideLabel: true
            }, items: [{
                xtype: 'combo', name: 'cominvoicecity', id: "cob_cominvoicecity", editable: false, queryMode: 'remote', border: false, displayField: 'middle', valueField: 'middlecode', store: InvoiceCityStore, allowBlank: false, width: 80,
                listeners: {
                    "select": function (combo, record) {
                        var Zip = Ext.getCmp("cob_cominvoicezip");
                        Zip.clearValue();
                        InvoiceZipStore.removeAll();
                        InvoiceComCity = true;
                    }
                }
            }, {
                xtype: 'combo', name: 'cominvoicezip', id: 'cob_cominvoicezip', editable: false, queryMode: 'remote', triggerAction: "all", border: false, displayField: 'small', valueField: 'zipcode', store: InvoiceZipStore, allowBlank: false, width: 100, margins: '0 5 0 5',
                listeners: {
                    beforequery: function (qe) {
                        if (InvoiceComCity) {
                            delete qe.combo.lastQuery;
                            InvoiceZipStore.load({
                                params: {
                                    topValue: Ext.getCmp("cob_cominvoicecity").getValue()
                                }
                            });
                            InvoiceComCity = false;
                        }
                    },
                    "select": function (combo, record) {
                        var ArrZip = Ext.getCmp("cob_cominvoicezip").getRawValue().split('/');
                        Ext.getCmp('txt_cominvoiceaddress').setValue(Ext.getCmp("cob_cominvoicecity").getRawValue() + ArrZip[1]);
                    }
                }
            }, { id: 'txt_cominvoiceaddress', xtype: 'textfield', width: 300, name: 'caddress', allowBlank: false }
            ]
        }]
    });

    Ext.create('Ext.form.Panel', {
        renderTo: 'txt_contractcreatedate', width: 150, border: false, readOnly: true,
        items: { id: 'txt_comcontractcreatedate', xtype: 'datefield', anchor: '95%', name: 'comcontractcreatedate', editable: false, format: 'Y/m/d', value: new Date() }
    });

    Ext.create('Ext.form.Panel', {
        renderTo: 'txt_contractstart', width: 150, border: false, readOnly: true,
        items: {
            id: 'txt_comcontractstart', xtype: 'datefield', anchor: '95%', name: 'comcontractstart', editable: false, format: 'Y/m/d', value: new Date(), maxText: DateTimeStart, listeners: {
                change: function (datefield, newValue, oldValue, eopts) {
                    var endDate = Ext.getCmp("txt_comcontractend");
                    endDate.setMinValue(newValue);
                    endDate.validate();
                }
            }
        }
    });

    Ext.create('Ext.form.Panel', {
        renderTo: 'txt_contractend', width: 150, border: false, readOnly: true,
        items: {
            id: 'txt_comcontractend', xtype: 'datefield', anchor: '95%', name: 'comcontractend', editable: false, format: 'Y/m/d', value: new Date(), minText: DateTimeEnd, listeners: {
                change: function (datefield, newValue, oldValue, eopts) {
                    var startDate = Ext.getCmp("txt_comcontractstart");
                    startDate.setMaxValue(newValue);
                    startDate.validate();
                }
            }
        }
    });

    Ext.create('Ext.form.Panel', {
        renderTo: 'txt_annaulfee', border: false,
        items: { id: 'txt_comannaulfee', xtype: 'numberfield', value: 0, step: 1, minValue: 0, width: 200, name: 'comannaulfee' }
    });

    Ext.create('Ext.form.Panel', {
        renderTo: 'txt_renewfee', border: false,
        items: { id: 'txt_comrenewfee', xtype: 'numberfield', value: 0, step: 1, minValue: 0, width: 200, name: 'comrenewfee' }
    });

    Ext.create('Ext.form.Panel', {
        renderTo: 'txt_managerEmail', border: false,
        items: { id: 'txt_commanagerEmail', xtype: 'textfield', width: 200, name: 'commanagerEmail', allowBlank: false }
    });

    Ext.create('Ext.form.ComboBox', {
        renderTo: 'div_modelIn', id: "cob_modelIn", editable: false, queryMode: 'local', displayField: 'text', valueField: 'value',
        store: ModelInStore, value: '1'
    });
    //add by xiangwang0413w 2014/06/26 增加 ERP客戶代號
    Ext.create('Ext.form.Panel', {
        renderTo: 'div_erpId', border: false,
        items: { id: 'txt_erpId', xtype: 'textfield', width: 200, name: 'erpId', allowBlank: false }
    });

    Ext.create('Ext.Button', {
        renderTo: 'btn_submit', iconCls: 'icon-add', text: SAVE, scale: 'large', style: { marginRight: '10px' },
        handler: Save
    });
    Ext.create('Ext.Button', {
        renderTo: 'btn_submit', iconCls: 'icon_rewind', text: RETURN, scale: 'large', style: { marginRight: '10px' }, hidden: window.parent.getChannelId() == '',
        handler: function () {
            window.parent.location.href = 'http://' + window.parent.location.host + '/channel/channellist';
        }
    });

    var channel_id = window.parent.getChannelId();
    if (channel_id != "") {
        Ext.Ajax.request({
            url: '/Channel/QueryChannelById',
            method: 'POST',
            params: {
                'channel_id': channel_id
            },
            success: function (response, opts) {
                var resText = eval("(" + response.responseText + ")");
                $("#rdo_chstatus [name='chstatus']").each(function () {
                    if ($(this).attr("value") == resText.items[0].channel_status) {
                        $(this).attr("checked", "true");
                    }
                });

                $("#rdo_chType [name='chType']").each(function () {
                    if ($(this).attr("value") == resText.items[0].channel_type) {
                        $(this).attr("checked", "true");
                    }
                });

                Ext.getCmp('txt_fname').setValue(resText.items[0].channel_name_full);
                Ext.getCmp('txt_sname').setValue(resText.items[0].channel_name_simple);
                Ext.getCmp('txt_cominvoice').setValue(resText.items[0].channel_invoice);
                Ext.getCmp('txt_comemail').setValue(resText.items[0].channel_email);

                /*Ext.getCmp('txt_cphone').setValue(resText.items[0].company_phone.split('-')[0]);
                Ext.getCmp('txt_cphonenum').setValue(resText.items[0].company_phone.split('-')[1]);
                Ext.getCmp('txt_cfax').setValue(resText.items[0].company_fax.split('-')[0]);
                Ext.getCmp('txt_cfaxnum').setValue(resText.items[0].company_fax.split('-')[1]);*/

                Ext.getCmp('txt_cphone').setValue(resText.items[0].company_phone.substring(0, 2));
                Ext.getCmp('txt_cphonenum').setValue(resText.items[0].company_phone.substring(2, resText.items[0].company_phone.length));
                Ext.getCmp('txt_cfax').setValue(resText.items[0].company_fax.substring(0, 2));
                Ext.getCmp('txt_cfaxnum').setValue(resText.items[0].company_fax.substring(2, resText.items[0].company_fax.length));

                CityStore.load({
                    callback: function (records, operation, success) {
                        var index = CityStore.find("middlecode", resText.items[0].companyCity);
                        if (index != -1) {
                            ZipStore.load({
                                params: {
                                    topValue: Ext.getCmp("cob_ccity").getValue(),
                                    topText: CityStore.getAt(index).get("middle")
                                }
                            });
                        }
                    }
                });
                Ext.getCmp('cob_ccity').setValue(resText.items[0].companyCity);

                if (resText.items[0].company_zip != 0) {
                    Ext.getCmp('cob_czip').setValue(resText.items[0].company_zip);
                }
                Ext.getCmp('txt_caddress').setValue(resText.items[0].company_address);
                Ext.getCmp('txt_cominvoicetitle').setValue(resText.items[0].invoice_title);

                InvoiceCityStore.load({
                    callback: function (records, operation, success) {
                        var index = InvoiceCityStore.find("middlecode", resText.items[0].invoiceCity);
                        if (index != -1) {
                            InvoiceZipStore.load({
                                params: {
                                    topValue: Ext.getCmp("cob_cominvoicecity").getValue(),
                                    topText: InvoiceCityStore.getAt(index).get("middle")
                                }
                            });
                        }
                    }
                });
                Ext.getCmp('cob_cominvoicecity').setValue(resText.items[0].invoiceCity);

                Ext.getCmp('cob_cominvoicezip').setValue(resText.items[0].invoice_zip);
                Ext.getCmp('txt_cominvoiceaddress').setValue(resText.items[0].invoice_address);
                Ext.getCmp('txt_comcontractcreatedate').setValue(resText.items[0].contract_createdate);
                Ext.getCmp('txt_comcontractstart').setValue(resText.items[0].contract_start);
                Ext.getCmp('txt_comcontractend').setValue(resText.items[0].contract_end);
                Ext.getCmp('txt_comannaulfee').setValue(resText.items[0].annaul_fee);
                Ext.getCmp('txt_comrenewfee').setValue(resText.items[0].renew_fee);
                Ext.getCmp('txt_commanagerEmail').setValue(resText.items[0].user_email);
                Ext.getCmp('cob_modelIn').setValue(resText.items[0].model_in);
                Ext.getCmp('txt_erpId').setValue(resText.items[0].erp_id);//add by xiangwang0413w 2014/06/26 增加 ERP客戶代號
                if (resText.items[0].notify_sms != 0) {
                    $("#ckb_notifysms").attr("checked", "true");
                }
            },
            failure: function (response) {
                var resText = eval("(" + response.responseText + ")");
            }
        });
    }
});

function ValedateForm() {
    if (!Ext.getCmp("txt_fname").isValid()) {
        alert(NOTNULL_FNAME);
        return false;
    }

    if (!Ext.getCmp("txt_sname").isValid()) {
        alert(NOTNULL_SNAME);
        return false;
    }

    if (!Ext.getCmp("txt_cominvoice").isValid()) {
        alert(NOTNULL_COMINVOICE);
        return false;
    }

    if (Ext.getCmp("txt_comemail").isValid()) {
        var reg = /\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$|^\s*$/;

        if (!reg.test(Ext.getCmp("txt_comemail").getValue())) {
            alert(ERROR_COMEMAIL);
            return false;
        }
    }

    if ((Ext.getCmp("txt_cphone").getValue() != null && Ext.getCmp("txt_cphonenum").getValue() == null) || (Ext.getCmp("txt_cphone").getValue() == null && Ext.getCmp("txt_cphonenum").getValue() != null)) {
        alert(ERROR_COMPHONE);
        return false;
    }

    if (Ext.getCmp("txt_caddress").getValue() != "") {
        if (Ext.getCmp("txt_caddress").getValue().replace(Ext.getCmp("cob_ccity").getRawValue() + Ext.getCmp("cob_czip").getRawValue().split('/')[1], '') == '') {
            alert(NOTNULL_COMADDRESS);
            return false;
        }
    }

    if (!Ext.getCmp("txt_cominvoicetitle").isValid()) {
        alert(NOTNULL_COMINVOICETITLE);
        return false;
    }

    if ((Ext.getCmp("txt_cfax").getValue() != null && Ext.getCmp("txt_cfaxnum").getValue() == null) || (Ext.getCmp("txt_cfax").getValue() == null && Ext.getCmp("txt_cfaxnum").getValue() != null)) {
        alert(ERROR_COMFAX);
        return false;
    }

    if (Ext.getCmp("cob_cominvoicecity").getValue() == null) {
        alert(NOTNULL_COMINVOICECITY);
        return false;
    }

    if (Ext.getCmp("cob_cominvoicezip").getValue() == null) {
        alert(NOTNULL_COMINVOICEZIP);
        return false;
    }
    if (Ext.getCmp("txt_cominvoiceaddress").getValue().replace(Ext.getCmp("cob_cominvoicecity").getRawValue() + Ext.getCmp("cob_cominvoicezip").getRawValue().split('/')[1], '') == '') {
        alert(NOTNULL_COMINVOICEADDRESS);
        return false;
    }

    if (!Ext.getCmp("txt_commanagerEmail").isValid()) {
        alert(NOTNULL_MANAGEREMAIL);
        return false;
    }
    else {
        var reg = /\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$|^\s*$/;

        if (!reg.test(Ext.getCmp("txt_commanagerEmail").getValue())) {
            alert(ERROR_MANAGEREMAIL);
            return false;
        }
    }
    return true;
}

function Save() {
    if (!ValedateForm()) {
        return;
    }
    var caddr = '', iaddr = '', notify_sms = 0;
    if (window.parent.getChannelId() == '') {
        caddr = Ext.getCmp("cob_ccity").getRawValue() + Ext.getCmp("cob_czip").getRawValue().split('/')[1] + Ext.getCmp('txt_caddress').getValue();
        iaddr = Ext.getCmp("cob_cominvoicecity").getRawValue() + Ext.getCmp("cob_cominvoicezip").getRawValue().split('/')[1] + Ext.getCmp('txt_cominvoiceaddress').getValue();
    }
    else {
        caddr = Ext.getCmp('txt_caddress').getValue();
        iaddr = Ext.getCmp('txt_cominvoiceaddress').getValue();
    }

    if ($("#ckb_notifysms").attr("checked")) {
        notify_sms = 1;
    }

    Ext.Ajax.request({
        url: '/Channel/SaveChannel',
        method: 'POST',
        params: {
            'channel_status': $("#rdo_chstatus [name='chstatus']:checked").val(),
            'channel_type': $("#rdo_chType [name='chType']:checked").val(),
            'channel_name_full': Ext.htmlEncode(Ext.getCmp('txt_fname').getValue()),
            'channel_name_simple': Ext.htmlEncode(Ext.getCmp('txt_sname').getValue()),
            'channel_invoice': Ext.htmlEncode(Ext.getCmp('txt_cominvoice').getValue()),
            'channel_email': Ext.htmlEncode(Ext.getCmp('txt_comemail').getValue()),
            'company_cphone': Ext.htmlEncode(Ext.getCmp('txt_cphone').getValue()),
            'company_cphonenum': Ext.htmlEncode(Ext.getCmp('txt_cphonenum').getValue()),
            'company_cfax': Ext.htmlEncode(Ext.getCmp('txt_cfax').getValue()),
            'company_cfaxnum': Ext.htmlEncode(Ext.getCmp('txt_cfaxnum').getValue()),
            'company_zip': Ext.htmlEncode(Ext.getCmp('cob_czip').getValue()),
            'company_address': Ext.htmlEncode(Ext.getCmp('txt_caddress').getValue()),
            'invoice_title': Ext.htmlEncode(Ext.getCmp('txt_cominvoicetitle').getValue()),
            'invoice_zip': Ext.htmlEncode(Ext.getCmp('cob_cominvoicezip').getValue()),
            'invoice_address': Ext.htmlEncode(Ext.getCmp('txt_cominvoiceaddress').getValue()),
            'contract_createdate': Ext.getCmp('txt_comcontractcreatedate').getValue(),
            'contract_start': Ext.getCmp('txt_comcontractstart').getValue(),
            'contract_end': Ext.getCmp('txt_comcontractend').getValue(),
            'annaul_fee': Ext.getCmp('txt_comannaulfee').getValue(),
            'renew_fee': Ext.getCmp('txt_comrenewfee').getValue(),
            'manager_Email': Ext.getCmp('txt_commanagerEmail').getValue(),
            'erp_id': Ext.getCmp('txt_erpId').getValue(),//add by xiangwang0413w 2014/06/26 增加 ERP客戶代號
            'model_in': Ext.getCmp('cob_modelIn').getValue(),
            'notify_sms': notify_sms,
            'channel_id': window.parent.getChannelId()
        },
        success: function (response, opts) {
            var resText = eval("(" + response.responseText + ")");
            window.parent.setChannelId(resText.channelId);
            alert(resText.msg);
        },
        failure: function (response) {
            var resText = eval("(" + response.responseText + ")");
            alert(resText.msg);
        }
    });
}

function Check(e) {
    if ($(e).is(":checked")) {
        if (Ext.getCmp("cob_ccity").getValue() == null || Ext.getCmp("cob_czip").getValue() == null || Ext.getCmp("txt_caddress").getValue() == null) {
            alert(NOTNULL_COMADDRESS);
            $(e).removeAttr("checked");
        }
        else {
            InvoiceCityStore.load();
            Ext.getCmp("cob_cominvoicecity").setValue(Ext.getCmp("cob_ccity").getValue());
            InvoiceZipStore.load({
                params: {
                    topValue: Ext.getCmp("cob_cominvoicecity").getValue()
                }
            });
            Ext.getCmp("cob_cominvoicezip").setValue(Ext.getCmp("cob_czip").getValue());
            Ext.getCmp("txt_cominvoiceaddress").setValue(Ext.getCmp("txt_caddress").getValue());
        }
    }
}