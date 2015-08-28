

//網頁Model
Ext.define("gigade.pageModel", {
    extend: 'Ext.data.Model',
    fields: [
        { name: 'page_id', type: 'string' },
        { name: 'page_name', type: 'string' }
    ]
});
//網頁Store
var pageidStore = Ext.create("Ext.data.Store", {
    model: 'gigade.pageModel',
    autoLoad: false,
    proxy: {
        type: 'ajax',
        url: '/WebContentType/GetPage',
        actionMethods: 'post',
        reader: {
            type: 'json',
            root: 'items'
        }
    }
});

Ext.define("gigade.areaModel", {
    extend: 'Ext.data.Model',
    fields: [
                    { name: 'area_id', type: 'string' },
                    { name: 'area_name', type: 'string' }
                ]
});
var areaidStore = Ext.create("Ext.data.Store", {
    model: 'gigade.areaModel',
    autoDestroy: false,
    autoLoad: false,
    proxy: {
        type: 'ajax',
        url: '/WebContentType/GetPage',
        actionMethods: 'post',
        reader: {
            type: 'json',
            root: 'items'
        }
    }
});
//連結類別Model
Ext.define("gigade.linkmode", {
    extend: 'Ext.data.Model',
    fields: [
        { name: 'link_mode', type: 'string' },
        { name: 'link_mode_name', type: 'string' }
    ]
});
//連結類別Store
var linkModelStore = Ext.create('Ext.data.Store', {
    model: 'gigade.linkmode',
    autoLoad: true,
    data: [
        { link_mode: '0', link_mode_name: '不開' },
        { link_mode: '1', link_mode_name: '開原視窗' },
        { link_mode: '2', link_mode_name: '開新視窗' }
    ]
});

//品牌Model
Ext.define("gigade.BrandModel", {
    extend: 'Ext.data.Model',
    fields: [
        { name: 'brand_id', type: 'string' },
        { name: 'brand_name', type: 'string' }
    ]
});
//品牌Store
var BrandStore = Ext.create("Ext.data.Store", {
    model: 'gigade.BrandModel',
    autoLoad: true,
    proxy: {
        type: 'ajax',
        url: '/WebContentType/QueryBrand',
        actionMethods: 'post',
        reader: {
            type: 'json',
            root: 'items'
        }
    }
});

//產品Model
Ext.define("gigade.ProductModel", {
    extend: 'Ext.data.Model',
    fields: [
        { name: 'product_id', type: 'string' },
        { name: 'product_name', type: 'string' }
    ]
});
//產品Store
var ProductStore = Ext.create("Ext.data.Store", {
    model: 'gigade.ProductModel',
    autoLoad: true,
    proxy: {
        type: 'ajax',
        url: '/WebContentType/QueryProduct',
        actionMethods: 'post',
        reader: {
            type: 'json',
            root: 'items'
        }
    }
});
function Tomorrow() {
    var d;
    var s = "";
    d = new Date();                             // 创建 Date 对象。
    s += d.getFullYear() + "/";                     // 获取年份。
    s += (d.getMonth() + 1) + "/";              // 获取月份。
    s += d.getDate() + 1;                          // 获取日。
    return (new Date(s));                                 // 返回日期。
};