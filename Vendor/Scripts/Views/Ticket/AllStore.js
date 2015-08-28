//供應商Model
Ext.define("gigade.Vendor", {
    extend: 'Ext.data.Model',
    fields: [
        { name: "vendor_id", type: "string" },
        { name: "vendor_name_simple", type: "string" }]
});
//供應商Store
var VendorStore = Ext.create('Ext.data.Store', {
    model: 'gigade.Vendor',
    autoLoad: true,
    proxy: {
        type: 'ajax',
        url: "/Ticket/GetVendor",
        actionMethods: 'post',
        reader: {
            type: 'json',
            root: 'data'
        }
    }
});

//付款單狀態
Ext.define('gigade.PaymentTypes', {
    extend: 'Ext.data.Model',
    fields: [
            { name: "ParameterCode", type: "string" },
         { name: "remark", type: "string" }
    ]
});
var paymentType = Ext.create('Ext.data.Store', {
    autoDestroy: true,
    model: 'gigade.PaymentTypes',
    proxy: {
        type: 'ajax',
        url: '/Ticket/GetPayMentType',
        actionMethods: 'post',
        reader: {
            type: 'json',
            root: 'data'
        }
    }
});

//付款方式
Ext.define("gigade.paraModel", {
    extend: 'Ext.data.Model',
    fields: [
        { name: 'parameterCode', type: 'string' },
        { name: 'parameterName', type: 'string' }
    ]
});
var paymentStore = Ext.create("Ext.data.Store", {
    model: 'gigade.paraModel',
    autoLoad: true,
    proxy: {
        type: 'ajax',
        url: '/Ticket/QueryPara?paraType=payment',
        noCache: false,
        getMethod: function () { return 'get'; },
        actionMethods: 'post',
        reader: {
            type: 'json',
            root: 'items'
        }
    }
});