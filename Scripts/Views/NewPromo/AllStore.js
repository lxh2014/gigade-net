//會員群組Model
Ext.define("gigade.VipGroup", {
    extend: 'Ext.data.Model',
    fields: [
        { name: "group_id", type: "string" },
        { name: "group_name", type: "string" }]
});
//會員群組（活動主表使用）
var VipGroupStoreEdit = Ext.create('Ext.data.Store', {
    model: 'gigade.VipGroup',
    autoLoad: false,
    proxy: {
        type: 'ajax',
        url: "/PromotionsDiscount/GetVipGroup",
        actionMethods: 'post',
        reader: {
            type: 'json',
            root: 'data'
        }

    }
});


//會員群組store（促銷共用表使用）
var VipGroupStore = Ext.create('Ext.data.Store', {
    model: 'gigade.VipGroup',
    autoLoad: false,
    proxy: {
        type: 'ajax',
        url: "/PromotionsDiscount/GetVipGroup",
        actionMethods: 'post',
        reader: {
            type: 'json',
            root: 'data'
        }

    }
});

var DDLStore = Ext.create('Ext.data.Store', {
    fields: ['txt', 'value'],
    data: [
        { "txt": PASSLIMIT, "value": "0" },
        { "txt": NOPASSLIMIT, "value": "1" }
    ]
});