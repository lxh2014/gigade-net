/*
所有在Promotions中使用到的Model以及Store
添加順序為Model-Store  一一對應
添加的時候注意命名規範
*/

Ext.define("gigade.Brand", {
    extend: 'Ext.data.Model',
    fields: [
    { name: "brand_id", type: "string" },
    { name: "brand_name", type: "string" }]
});




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
    autoLoad: true,
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


//site
Ext.define("gigade.Site", {
    extend: 'Ext.data.Model',
    fields: [
        { name: "Site_Id", type: "string" },
        { name: "Site_Name", type: "string" }]
});

var SiteStore = Ext.create('Ext.data.Store', {
    model: 'gigade.Site',
    autoLoad: true,
    proxy: {
        type: 'ajax',
        url: "/PromotionsDiscount/GetSite",
        actionMethods: 'post',
        reader: {
            type: 'json',
            root: 'data'
        }

    }
});
function Tomorrow() {
    var d;
    //var s = "";
    d = new Date();                             // 创建 Date 对象。
    // s += d.getFullYear() + "/";                     // 获取年份。
    //s += (d.getMonth() + 1) + "/";              // 获取月份。
    //s += d.getDate() + 1;                          // 获取日。
    //return (new Date(s));                                 // 返回日期。
    d.setDate(d.getDate() + 1);
    //alert(d);
    return d;
}

//前台分類store
var frontCateStore = Ext.create('Ext.data.TreeStore', {
    autoLoad: true,
    proxy: {
        type: 'ajax',
        url: '/ProductList/GetFrontCatagory',
        actionMethods: 'post'
    },
    root: {
        expanded: true,
        checked: false,
        children: [

        ]
    }
});
frontCateStore.load();




//function BounTypeShow(val) {
//    switch (val) {
//        case "1":
//            return BONUS_ONE;
//            break;
//        case "2":
//            return BONUS_TWO;
//            break;
//        case "3":
//            return BONUS_THREE;
//            break;
//        case "4":
//            return BONUS_FOUR;
//            break;
//        case "0":
//            return DEVICE_1;
//            break;
//    }
//}

//function ZhiFuType(val) {
//    switch (val) {
//        case "":
//            return DEVICE_1;
//            break;
//        default:
//            return val;
//            break;
//    }
//}
//館別Model
Ext.define("gigade.ShopClass", {
    extend: 'Ext.data.Model',
    fields: [
        { name: "class_id", type: "string" },
        { name: "class_name", type: "string" }]
});
//館別Store
var ShopClassStore = Ext.create('Ext.data.Store', {
    model: 'gigade.ShopClass',
    autoLoad: true,
    proxy: {
        type: 'ajax',
        url: "/PromotionsMaintain/GetShopClass",
        actionMethods: 'post',
        reader: {
            type: 'json',
            root: 'data'
        }
    }
});



var ShopClassStoreYuan = Ext.create('Ext.data.Store', {
    model: 'gigade.ShopClass',
    // autoDestroy: true,
    autoLoad: true,
    proxy: {
        type: 'ajax',
        url: "/PromotionsMaintain/GetShopClass",
        actionMethods: 'post',
        reader: {
            type: 'json',
            root: 'data'
        }
    }
});
/*********參數表model***********/
Ext.define("gigade.paraModel", {
    extend: 'Ext.data.Model',
    fields: [
        { name: 'parameterCode', type: 'string' },
        { name: 'parameterName', type: 'string' }
    ]
});

//裝置
var deviceStore = Ext.create("Ext.data.Store", {
    model: 'gigade.paraModel',
    autoLoad: true,
    proxy: {
        type: 'ajax',
        url: '/Parameter/QueryPara?paraType=device',
        noCache: false,
        getMethod: function () { return 'get'; },
        actionMethods: 'post',
        reader: {
            type: 'json',
            root: 'items'
        }
    }
});
//付款方式

var paymentStore = Ext.create("Ext.data.Store", {
    model: 'gigade.paraModel',
    autoLoad: true,
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


//運送類別Model
//Ext.define("gigade.YunsongModel", {
//    extend: 'Ext.data.Model',
//    fields: [
//        { name: "type_id", type: "string" },
//        { name: "type_name", type: "string" }]
//});
////運送類別Store
//var YunsongStore = Ext.create('Ext.data.Store', {
//   // model: 'gigade.YunsongModel',
// //   autoLoad: true,
//    data: [
//        { type_id: '0', type_name: "不分" },
//        { type_id: '1', type_name: "常溫" },
//        { type_id: '2', type_name: "冷凍" }
//    ]
//});








//物流方式Model
Ext.define("gigade.DeliverModel", {
    extend: 'Ext.data.Model',
    fields: [
        { name: "deliver_id", type: "string" },
        { name: "deliver_name", type: "string" }]
});
//物流方式Store
var DeliverStore = Ext.create('Ext.data.Store', {
    model: 'gigade.DeliverModel',
    autoLoad: true,
    data: [
        { deliver_id: '1', deliver_name: DELIVERDEFAULT }
    ]
});
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
        url: "/Vendor/GetVendor",
        actionMethods: 'post',
        reader: {
            type: 'json',
            root: 'data'
        }
    }
});
//過期未過期的store
//var ExpireZheStore = Ext.create('Ext.data.Store', {
//    fields: ['txt', 'value'],
//    data: [
//    { "txt": NOTPASTDUE, "value": "0" },
//    { "txt": TEXPIRED, "value": "1" }
//    ]
//})
//var UrlZheStore = Ext.create('Ext.data.Store', {
//    fields: ['txt', 'value'],
//    data: [
//    { "txt": "否", "value": "0" },
//    { "txt": "是", "value": "1" }
//    ]
//})
