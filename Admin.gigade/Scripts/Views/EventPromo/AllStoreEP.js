/*
注意：寫在該文件中的store一定要保證屬性AutoStore:false;
如要調用該store，需在調用的文件中對該store進行load();
*/
//會員群組Model
Ext.define("gigade.VipGroup", {
    extend: 'Ext.data.Model',
    fields: [
        { name: "group_id", type: "string" },
        { name: "group_name", type: "string" }]
});
//會員群組store
var VipGroupStore = Ext.create('Ext.data.Store', {
    model: 'gigade.VipGroup',
    //  autoLoad: true,
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

//會員等級Model
Ext.define("gigade.UserLevel", {
    extend: 'Ext.data.Model',
    fields: [
    { name: "ml_code", type: "string" },
    { name: "ml_name", type: "string" }
    ]
});

//會員等級store
var userLevelStore = Ext.create("Ext.data.Store", {
    model: "gigade.UserLevel",
    proxy: {
        type: 'ajax',
        url: "/MemberEvent/GetMemberLevelDownList",
        actionMethods: 'post',
        reader: {
            type: 'json',
            root: 'data'
        }
    }
});

//站台Model
Ext.define("gigade.Site", {
    extend: 'Ext.data.Model',
    fields: [
        { name: "Site_Id", type: "string" },
        { name: "Site_Name", type: "string" }]
});

////站台Store
var SiteStore = Ext.create('Ext.data.Store', {
    model: 'gigade.Site',
    proxy: {
        type: 'ajax',
        url: "/SiteManager/GetSiteStore",
        actionMethods: 'post',
        reader: {
            type: 'json',
            root: 'data'
        }

    }
});



//活動會員條件Model
Ext.define("gigade.CondiType", {
    extend: 'Ext.data.Model',
    fields: [
    { name: "condi_id", type: "string" },
    { name: "condi_name", type: "string" }
    ]
});

//活動會員條件store
var UserConStore = Ext.create('Ext.data.Store', {
    model: 'gigade.CondiType',
    proxy: {
        type: 'ajax',
        url: "/EventPromo/GetEventCondiUser",
        //    autoDestroy: true,
        actionMethods: 'post',
        reader: {
            type: 'json',
            root: 'data'
        }

    }
});
//參與活動的對象類型
var ConTypeStore = Ext.create('Ext.data.Store', {
    fields: ['condi_id', 'condi_name'],
    data: [
        { condi_id: '1', condi_name: "按品牌" },
        { condi_id: '2', condi_name: "按類別" },
        { condi_id: '3', condi_name: "按館別" },
        { condi_id: '4', condi_name: "按商品" },
        { condi_id: '5', condi_name: "按購物車" },
        { condi_id: '6', condi_name: "按付款方式" }
    ]
});

//贈品類型
var GiftTypeStore = Ext.create('Ext.data.Store', {
    fields: ['text', 'value'],
    data: [
        { value: '1', text: "商品" },
        { value: '2', text: "購物金" },
        { value: '3', text: "抵用券" }

    ]
});
//品牌Model
Ext.define("gigade.Brand", {
    extend: 'Ext.data.Model',
    fields: [
    { name: "brand_id", type: "string" },
    { name: "brand_name", type: "string" }
    ]
});

//品牌Store
var BrandStore = Ext.create('Ext.data.Store', {
    model: 'gigade.Brand',
    proxy: {
        type: 'ajax',
        url: "/EventPromo/QueryBrand",
        actionMethods: 'post',
        reader: {
            type: 'json',
            root: 'item'
        }

    }
});



//館別Model
Ext.define("gigade.ShopClass", {
    extend: 'Ext.data.Model',
    fields: [
    { name: "class_id", type: "string" },
    { name: "class_name", type: "string" }
    ]
});

//館別Store
var ShopClassStore = Ext.create('Ext.data.Store', {
    model: 'gigade.ShopClass',
    proxy: {
        type: 'ajax',
        url: "/EventPromo/QueryShopClass",
        actionMethods: 'post',
        reader: {
            type: 'json',
            root: 'data'
        }

    }
});

//參數表Model
Ext.define("gigade.paraModel", {
    extend: 'Ext.data.Model',
    fields: [
        { name: 'parameterCode', type: 'string' },
        { name: 'parameterName', type: 'string' }
    ]
});
//付款方式Store

var PaymentStore = Ext.create("Ext.data.Store", {
    model: 'gigade.paraModel',
    proxy: {
        type: 'ajax',
        url: '/Parameter/QueryPara?paraType=payment',
        noCache: false,
        getMethod: function () { return 'get'; },
        actionMethods: 'post',
        reader: {
            type: 'json',
            root: 'items'
        }
    }
});

//購物車Model
Ext.define("gigade.ShopCart", {
    extend: 'Ext.data.Model',
    fields: [
    { name: "cart_id", type: "string" },
    { name: "cart_name", type: "string" }
    ]
});

//購物車Store
var ShopCartStore = Ext.create('Ext.data.Store', {
    model: 'gigade.ShopCart',
    proxy: {
        type: 'ajax',
        url: "/EventPromo/QueryShopCart",
        actionMethods: 'post',
        reader: {
            type: 'json',
            root: 'data'
        }

    }
});


var frontCateStore = Ext.create('Ext.data.TreeStore', {
    autoLoad: true,
    proxy: {
        type: 'ajax',
        url: '/EventPromo/GetFrontCatagory',
        actionMethods: 'post'
    },
    root: {
        children: []
    }
});
var treeCateStore = Ext.create('Ext.data.TreeStore', {
    autoLoad: true,
    proxy: {
        type: 'ajax',
        url: '/Product/GetProductCatagory',
        actionMethods: 'post'
    },
    rootVisible: false,
    root: {
        text: '商品類別',
        expanded: true,
        children: []
    }
});


Ext.define('GIGADE.EVENTPRODUCT', {
    extend: 'Ext.data.Model',
    fields: [
      { name: 'product_id', type: 'int' },
      { name: 'product_name', type: 'string' },
      { name: 'product_num', type: 'int' }
    ]
});

var ac_store = Ext.create('Ext.data.Store', {
    model: 'GIGADE.EVENTPRODUCT'
});

//輔助方法
setNextMonth = function (source, n) {
    var s = new Date(source);
    s.setMonth(s.getMonth() + n);
    if (n < 0) {
        s.setHours(0, 0, 0);
    }
    else if (n > 0) {
        s.setHours(23, 59, 59);
    }
    return s;
}