var channelId = '';
var bypercent = 1;

//其他諮詢Model
Ext.define('MainMessage', {
    extend: 'Ext.data.Model',
    fields: [
        { name: "deal_method", type: "string" },
        { name: "deal_percent", type: "string" },
        { name: "deal_fee", type: "string" },
        { name: "creditcard_1_percent", type: "string" },
        { name: "creditcard_3_percent", type: "string" },
        { name: "shopping_car_percent", type: "string" },
        { name: "commission_percent", type: "string" },
        { name: "invoice_checkout_day", type: "string" },
        { name: "invoice_apply_start", type: "string" },
        { name: "invoice_apply_end", type: "string" },
        { name: "checkout_note", type: "string" },
        { name: "receipt_to", type: "string" },
        { name: "channel_manager", type: "string" },
        { name: "channel_note", type: "string" }]
});

var OutDayStore = Ext.create('Ext.data.Store', {
    fields: ['text', 'value'],
    data: [
        { "text": "1", "value": "1" },
        { "text": "2", "value": "2" },
        { "text": "3", "value": "3" },
        { "text": "4", "value": "4" },
        { "text": "5", "value": "5" },
        { "text": "6", "value": "6" },
        { "text": "7", "value": "7" },
        { "text": "8", "value": "8" },
        { "text": "9", "value": "9" },
        { "text": "10", "value": "10" },
        { "text": "11", "value": "11" },
        { "text": "12", "value": "12" },
        { "text": "13", "value": "13" },
        { "text": "14", "value": "14" },
        { "text": "15", "value": "15" },
        { "text": "16", "value": "16" },
        { "text": "17", "value": "17" },
        { "text": "18", "value": "18" },
        { "text": "19", "value": "19" },
        { "text": "20", "value": "20" },
        { "text": "21", "value": "21" },
        { "text": "22", "value": "22" },
        { "text": "23", "value": "23" },
        { "text": "24", "value": "24" },
        { "text": "25", "value": "25" },
        { "text": "26", "value": "26" },
        { "text": "27", "value": "27" },
        { "text": "28", "value": "28" },
        { "text": "29", "value": "29" },
        { "text": "30", "value": "30" },
        { "text": "31", "value": "31" }
    ]
});

var ApplyStartStore = Ext.create('Ext.data.Store', {
    fields: ['text', 'value'],
    data: [
        { "text": "1", "value": "1" },
        { "text": "2", "value": "2" },
        { "text": "3", "value": "3" },
        { "text": "4", "value": "4" },
        { "text": "5", "value": "5" },
        { "text": "6", "value": "6" },
        { "text": "7", "value": "7" },
        { "text": "8", "value": "8" },
        { "text": "9", "value": "9" },
        { "text": "10", "value": "10" },
        { "text": "11", "value": "11" },
        { "text": "12", "value": "12" },
        { "text": "13", "value": "13" },
        { "text": "14", "value": "14" },
        { "text": "15", "value": "15" },
        { "text": "16", "value": "16" },
        { "text": "17", "value": "17" },
        { "text": "18", "value": "18" },
        { "text": "19", "value": "19" },
        { "text": "20", "value": "20" },
        { "text": "21", "value": "21" },
        { "text": "22", "value": "22" },
        { "text": "23", "value": "23" },
        { "text": "24", "value": "24" },
        { "text": "25", "value": "25" },
        { "text": "26", "value": "26" },
        { "text": "27", "value": "27" },
        { "text": "28", "value": "28" },
        { "text": "29", "value": "29" },
        { "text": "30", "value": "30" },
        { "text": "31", "value": "31" }
    ]
});

var ApplyEndStore = Ext.create('Ext.data.Store', {
    fields: ['text', 'value'],
    data: [
        { "text": "1", "value": "1" },
        { "text": "2", "value": "2" },
        { "text": "3", "value": "3" },
        { "text": "4", "value": "4" },
        { "text": "5", "value": "5" },
        { "text": "6", "value": "6" },
        { "text": "7", "value": "7" },
        { "text": "8", "value": "8" },
        { "text": "9", "value": "9" },
        { "text": "10", "value": "10" },
        { "text": "11", "value": "11" },
        { "text": "12", "value": "12" },
        { "text": "13", "value": "13" },
        { "text": "14", "value": "14" },
        { "text": "15", "value": "15" },
        { "text": "16", "value": "16" },
        { "text": "17", "value": "17" },
        { "text": "18", "value": "18" },
        { "text": "19", "value": "19" },
        { "text": "20", "value": "20" },
        { "text": "21", "value": "21" },
        { "text": "22", "value": "22" },
        { "text": "23", "value": "23" },
        { "text": "24", "value": "24" },
        { "text": "25", "value": "25" },
        { "text": "26", "value": "26" },
        { "text": "27", "value": "27" },
        { "text": "28", "value": "28" },
        { "text": "29", "value": "29" },
        { "text": "30", "value": "30" },
        { "text": "31", "value": "31" }
    ]
});

var ReceiptToStore = Ext.create('Ext.data.Store', {
    fields: ['text', 'value'],
    data: [
        { "text": RECEIPTTO_ONE, "value": "1" },
        { "text": RECEIPTTO_TWO, "value": "2" },
        { "text": RECEIPTTO_THREE, "value": "3" }
    ]
});

Ext.onReady(function () {
    $("#main").height(document.documentElement.clientHeight - 5);

    Ext.create('Ext.form.Panel', { renderTo: 'div_dealpercent', border: false,
        items: [{ xtype: 'fieldcontainer', fieldLabel: '', msgTarget: 'under', layout: 'hbox',
            defaults: {
                hideLabel: true
            },
            items: [{ xtype: 'displayfield', value: CHENGJIAO },
            { id: 'txt_dealpercent', xtype: 'textfield', width: 80, name: 'dealpercent', margins: '0 5 0 5' },
            { xtype: 'displayfield', value: '%'}]
        }]
    });

    $("#div_dealpercent").hide();


    Ext.create('Ext.form.Panel', { renderTo: 'div_dealfee', border: false,
        items: [{ xtype: 'fieldcontainer', fieldLabel: '', msgTarget: 'under', layout: 'hbox',
            defaults: {
                hideLabel: true
            },
            items: [{ xtype: 'displayfield', value: MEIBI },
            { id: 'txt_dealfee', xtype: 'numberfield', value: 0, step: 1, minValue: 0, width: 80, name: 'dealfee', margins: '0 5 0 5' },
            { xtype: 'displayfield', value: YUAN}]
        }]
    });
    $("#div_dealfee").hide();

    Ext.create('Ext.form.Panel', { renderTo: 'div_creditcardpercent1', border: false, items: [{ xtype: 'fieldcontainer', fieldLabel: '', msgTarget: 'under', layout: 'hbox',
        defaults: {
            hideLabel: true
        },
        items: [{ id: 'txt_creditcardpercent1', xtype: 'textfield', width: 80, name: 'creditcardpercent1' },
            { xtype: 'displayfield', value: '%', margins: '0 5 0 5'}]
    }]
    });

    Ext.create('Ext.form.Panel', { renderTo: 'div_creditcardpercent3', border: false, items: [{ xtype: 'fieldcontainer', fieldLabel: '', msgTarget: 'under', layout: 'hbox',
        defaults: {
            hideLabel: true
        },
        items: [{ id: 'txt_creditcardpercent3', xtype: 'textfield', width: 80, name: 'creditcardpercent3' },
                    { xtype: 'displayfield', value: '%', margins: '0 5 0 5'}]
    }]
    });

    Ext.create('Ext.form.Panel', { renderTo: 'div_shoppingcarpercent', border: false, items: [{ xtype: 'fieldcontainer', fieldLabel: '', msgTarget: 'under', layout: 'hbox',
        defaults: {
            hideLabel: true
        },
        items: [{ id: 'txt_shoppingcarpercent', xtype: 'numberfield', value: 0, step: 1, minValue: 0, width: 80, name: 'shoppingcarpercent' },
                    { xtype: 'displayfield', value: YUAN, margins: '0 5 0 5'}]
    }]
    });

    Ext.create('Ext.form.Panel', { renderTo: 'div_commissionpercent', border: false, items: [{ xtype: 'fieldcontainer', fieldLabel: '', msgTarget: 'under', layout: 'hbox',
        defaults: {
            hideLabel: true
        },
        items: [{ id: 'txt_commissionpercent', xtype: 'textfield', width: 80, name: 'commissionpercent' },
                    { xtype: 'displayfield', value: '%',margins: '0 5 0 5'}]
    }]
    });

    Ext.create('Ext.form.Panel', { renderTo: 'div_costby', border: false,
        items: [{ xtype: 'fieldcontainer', fieldLabel: '', msgTarget: 'under', layout: 'hbox',
            defaults: {
                hideLabel: true
            },
            items: [{ xtype: 'displayfield', value: SHIPPINGTYPE_ROOM },
            { id: 'txt_costlowpercent', xtype: 'textfield', width: 30, name: 'dealfee', margins: '0 5 0 5' },
            { xtype: 'displayfield', value: '%' },
            { xtype: 'displayfield', value: SHIPPINGTYPE_ICE },
            { id: 'txt_costnormalpercent', xtype: 'textfield', width: 30, name: 'dealfee', margins: '0 5 0 5' },
            { xtype: 'displayfield', value: '%'}]
        }]
    });

    $("#div_costby").hide();

    Ext.create('Ext.form.ComboBox', { renderTo: 'div_invoicecheckoutday', id: 'cob_invoicecheckoutday', editable: false, queryMode: 'local', displayField: 'text', valueField: 'value', store: OutDayStore
    });

    Ext.create('Ext.form.Panel', { renderTo: 'div_invoiceapply', border: false, width: 500,
        items: [{ xtype: 'fieldcontainer', fieldLabel: '', msgTarget: 'under', layout: 'hbox',
            defaults: {
                hideLabel: true
            },
            items: [{ id: 'cob_invoiceapplystart', xtype: 'combo', name: 'invoiceapplystart', editable: false, queryMode: 'local', displayField: 'text', valueField: 'value', store: ApplyStartStore },
                { xtype: 'displayfield', value: '~', margins: '0 5 0 5' },
                { id: 'cob_invoiceapplyend', xtype: 'combo', name: 'invoiceapplyend', editable: false, queryMode: 'local', displayField: 'text', valueField: 'value', store: ApplyEndStore}]
    }]
    });

    Ext.create('Ext.form.Panel', { renderTo: 'div_checkoutnote', border: false,
        items: { id: 'txt_checkoutnote', xtype: 'textfield', width: 200, name: 'checkoutnote' }
    });

    Ext.create('Ext.form.ComboBox', { renderTo: 'div_receiptto', id: "cob_receiptto", editable: false, queryMode: 'local', displayField: 'text', valueField: 'value', store: ReceiptToStore
    });

    Ext.create('Ext.form.Panel', { renderTo: 'div_channelmanager', border: false,
        items: { id: 'txt_channelmanager', xtype: 'textfield', width: 100, name: 'channelmanager' }
    });

    Ext.create('Ext.form.Panel', { renderTo: 'div_channelnote', border: false,
        items: { id: 'txt_channelnote', xtype: 'textareafield', width: 500, name: 'channelnote' }
    });

    Ext.create('Ext.Button', { renderTo: 'btn_submit', id: 'btn_sub', iconCls: 'icon-add', text: SAVE, scale: 'large', style: { marginRight: '10px' },
        handler: Save
    });
    Ext.create('Ext.Button', { renderTo: 'btn_submit', iconCls: 'icon_rewind', text: RETURN, scale: 'large', style: { marginRight: '10px' }, hidden: window.parent.getChannelId() == '',
        handler: function () {
            window.parent.location.href = 'http://' + window.parent.location.host + '/channel/channellist';
        }
    });

    channelId = window.parent.getChannelId();
    if (channelId == "") {
        Ext.getCmp("btn_sub").setDisabled(true);
    }
    else {
        Ext.Ajax.request({
            url: '/Channel/QueryOther',
            method: 'POST',
            params: {
                'channel_id': channelId
            },
            success: function (response, opts) {
                var resText = eval("(" + response.responseText + ")");
                $("#rdo_dealmethod [name='method']").each(function () {
                    if ($(this).attr("value") == resText.items[0].deal_method) {
                        $(this).attr("checked", "true");
                    }
                });
                if (resText.items[0].deal_method == '1') {
                    $("#div_dealpercent").hide();
                    $("#div_dealfee").hide();
                }
                else if (resText.items[0].deal_method == '2') {
                    $("#div_dealpercent").show();
                    $("#div_dealfee").hide();
                }
                else if (resText.items[0].deal_method == '3') {
                    $("#div_dealpercent").hide();
                    $("#div_dealfee").show();
                }
                Ext.getCmp('txt_dealpercent').setValue(resText.items[0].deal_percent);
                Ext.getCmp('txt_dealfee').setValue(resText.items[0].deal_fee);
                Ext.getCmp('txt_creditcardpercent1').setValue(resText.items[0].creditcard_1_percent);
                Ext.getCmp('txt_creditcardpercent3').setValue(resText.items[0].creditcard_3_percent);
                Ext.getCmp('txt_shoppingcarpercent').setValue(resText.items[0].shopping_car_percent);
                Ext.getCmp('txt_commissionpercent').setValue(resText.items[0].commission_percent);
                if (resText.items[0].cost_by_percent == '2') {
                    $("#ckb_costbypercent").attr("checked", "checked");
                    $("#div_costby").show();
                }
                Ext.getCmp('txt_costlowpercent').setValue(resText.items[0].cost_low_percent);
                Ext.getCmp('txt_costnormalpercent').setValue(resText.items[0].cost_normal_percent);
                Ext.getCmp('cob_invoicecheckoutday').setValue(resText.items[0].invoice_checkout_day);
                Ext.getCmp('cob_invoiceapplystart').setValue(resText.items[0].invoice_apply_start);
                Ext.getCmp('cob_invoiceapplyend').setValue(resText.items[0].invoice_apply_end);
                Ext.getCmp('txt_checkoutnote').setValue(resText.items[0].checkout_note);
                Ext.getCmp('cob_receiptto').setValue(resText.items[0].receipt_to);
                Ext.getCmp('txt_channelmanager').setValue(resText.items[0].channel_manager);
                Ext.getCmp('txt_channelnote').setValue(resText.items[0].channel_note);
            },
            failure: function (response) {
                var resText = eval("(" + response.responseText + ")");
            }
        });
    }

    $("#rdo_dealmethod").children().each(function () {
        $(this).change(function () {
            switch (this.id) {
                case "rdo_method1":
                    $("#div_dealpercent").hide();
                    $("#div_dealfee").hide();
                    break;
                case "rdo_method2":
                    $("#div_dealpercent").show();
                    $("#div_dealfee").hide();
                    break;
                case "rdo_method3":
                    $("#div_dealpercent").hide();
                    $("#div_dealfee").show();
                    break;
                default:
                    break;
            }
        });
    });
});

function Check(e) {
    if ($(e).is(":checked")) {
        $("#div_costby").show();
    }
    else {
        $("#div_costby").hide();
    }
}

function setDisable(flag) {
    Ext.getCmp("btn_sub").setDisabled(flag);
}

function ValedateForm() {
    if (window.parent.getChannelId() == "") {
        alert(NEXT_INSERT);
        return false;
    }
    $("#rdo_dealmethod input").each(function () {
        if ($(this)[0].checked) {
            if ($(this).val() == "2") {
                if (Ext.getCmp("txt_dealpercent").getValue() == null || Ext.getCmp("txt_dealpercent").getValue() == '0') {
                    alert(NOTNULL_DEALPERCENT);
                    return false;
                }
            }
            if ($(this).val() == "3") {
                if (Ext.getCmp("txt_dealfee").getValue() == null || Ext.getCmp("txt_dealfee").getValue() == '0') {
                    alert(NOTNULL_DEALFEE);
                    return false;
                }
            }
        }
    });

    if ($("#ckb_costbypercent").attr("checked")) {
        if (Ext.getCmp('txt_costlowpercent').getValue() == null && Ext.getCmp('txt_costnormalpercent').getValue() == null)
        {
            alert(NOTNULL_COSTBYPERCENT);
            return false;
        }
        bypercent = 2;
    }
    
    return true;
}

function Save() {
    if (!ValedateForm()) {
        return;
    }

    Ext.Ajax.request({
        url: '/Channel/SaveChannelOther',
        method: 'POST',
        params: {
            'channel_id': window.parent.getChannelId(),
            'deal_method': $("#rdo_dealmethod [name='method']:checked").val(),
            'deal_percent': Ext.getCmp('txt_dealpercent').getValue(),
            'deal_fee': Ext.getCmp('txt_dealfee').getValue(),
            'creditcard_1_percent': Ext.getCmp('txt_creditcardpercent1').getValue(),
            'creditcard_3_percent': Ext.getCmp('txt_creditcardpercent3').getValue(),
            'shopping_car_percent': Ext.getCmp('txt_shoppingcarpercent').getValue(),
            'commission_percent': Ext.getCmp('txt_commissionpercent').getValue(),
            'cost_by_percent': bypercent,
            'cost_low_percent': Ext.getCmp('txt_costlowpercent').getValue(),
            'cost_normal_percent': Ext.getCmp('txt_costnormalpercent').getValue(),
            'invoice_checkout_day': Ext.getCmp('cob_invoicecheckoutday').getValue(),
            'invoice_apply_start': Ext.getCmp('cob_invoiceapplystart').getValue(),
            'invoice_apply_end': Ext.getCmp('cob_invoiceapplyend').getValue(),
            'checkout_note': Ext.htmlEncode(Ext.getCmp('txt_checkoutnote').getValue()),
            'receipt_to': Ext.getCmp('cob_receiptto').getValue(),
            'channel_manager': Ext.htmlEncode(Ext.getCmp('txt_channelmanager').getValue()),
            'channel_note': Ext.htmlEncode(Ext.getCmp('txt_channelnote').getValue())
        },
        success: function (response, opts) {
            var resText = eval("(" + response.responseText + ")");
            alert(resText.msg);
        },
        failure: function (response) {
            var resText = eval("(" + response.responseText + ")");
            alert(resText.msg);
        }
    });
} 