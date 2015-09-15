
var flag = false;

Ext.define("gigade.DeliveryStore", {
    extend: 'Ext.data.Model',
    fields: [
        { name: "rowid", type: "int" },
        { name: "delivery_store_id", type: "int" },
        { name: "store_id", type: "string" },
        { name: "store_name", type: "string" }]
});


Ext.define("gigade.Parameter", {
    extend: 'Ext.data.Model',
    fields: [
        { name: "rowid", type: "int" },
        { name: "parameterType", type: "string" },
        { name: "parameterCode", type: "string" },
        { name: "parameterName", type: "string" }]
});


Ext.define("gigade.Parameter2", {
    extend: 'Ext.data.Model',
    fields: [
        { name: "Rowid", type: "int" },
        { name: "ParameterType", type: "string" },
        { name: "ParameterCode", type: "string" },
        { name: "parameterName", type: "string" }]
});


var DeliveryStore = Ext.create('Ext.data.Store', {
    model: 'gigade.DeliveryStore',
    autoLoad: true,
    proxy: {
        type: 'ajax',
        url: "/DeliveryStore/GetDelieveryStoreInfo",
        noCache: false,
        actionMethods: 'post',
        reader: {
            type: 'json',
            root: 'item'
        }
    }
});
var FreightTypeStore = Ext.create('Ext.data.Store', {
    proxy: {
        type: 'ajax',
        url: "/DeliveryStore/GetProductDeliverySetById",
        actionMethods: 'post'
    },
    model: 'gigade.Parameter2'
});

FreightTypeStore.on('beforeload', function () {
    Ext.apply(FreightTypeStore.proxy.extraParams,
    {
        rangeid: Ext.getCmp("freight_big_area").getValue()
    })
});

var ProductDeliverySetStore = Ext.create('Ext.data.Store', {
    model: 'gigade.Parameter',
    autoLoad: true,
    proxy: {
        type: 'ajax',
        url: "/DeliveryStore/GetProductDeliverySet",
        noCache: false,
        actionMethods: 'post',
        reader: {
            type: 'json',
            root: 'items'
        }
    }
});

var delieverStore = {
    xtype: 'combobox',
    fieldLabel: LOGISTICS_COMPANY,
    store: DeliveryStore,
    id: 'delivery_store_id',
    colName: 'delivery_store_id',
    //hidden: true,
    name: 'delivery_store_id',
    displayField: 'store_name',
    valueField: 'delivery_store_id'
};

var delieverRange = {
    xtype: 'combobox',
    fieldLabel: DELIVER_REGION,
    store: ProductDeliverySetStore,
    id: 'freight_big_area',
    colName: 'freight_big_area',
    //hidden: true,
    name: 'freight_big_area',
    displayField: 'parameterName',
    queryMode: 'local',
    editable: false,
    valueField: 'parameterCode',
    listeners: {
        "select": function (combo, record) {
            var z = Ext.getCmp("freight_type");
            z.clearValue();
            FreightTypeStore.removeAll();
        },
        beforequery: function (qe) {
            //ProductDeliverySetStore.load();
        }
    }
};

var freightType = {
    xtype: 'combobox',
    fieldLabel: LOGISTICS_DISTRMODE,
    store: FreightTypeStore,
    id: 'freight_type',
    colName: 'freight_type',
    //hidden: true,
    name: 'Freight_type',
    displayField: 'parameterName',
    valueField: 'ParameterCode',
    queryMode: 'local',
    editable: false,

    listeners: {
        beforequery: function (qe) {
            if (FreightTypeStore.data.length > 0) {
                return;
            }
            delete qe.combo.lastQuery;
            FreightTypeStore.load({
                params: {
                    rangeid: Ext.getCmp("freight_big_area").getValue()
                }
            });
        }
    }

};

function getFreightType() {
    //獲取code值
    var rangeid = Ext.htmlEncode(Ext.getCmp("freight_big_area").getValue());
    Ext.getCmp("freight_type").setValue("");
    //創建動態數據store
    FreightTypeStore.load({ params: { 'rangeid': rangeid } });
}


