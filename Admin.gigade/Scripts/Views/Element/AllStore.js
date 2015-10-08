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
        url: "/Element/GetSite",
        actionMethods: 'post',
        reader: {
            type: 'json',
            root: 'data'
        }

    }
});

Ext.define("gigade.page", {
    extend: 'Ext.data.Model',
    fields: [
    { name: "page_id", type: "string" },
    { name: "page_name", type: "string" }]
});

/*********參數表model***********/
Ext.define("gigade.paraModel", {
    extend: 'Ext.data.Model',
    fields: [
        { name: 'parameterCode', type: 'string' },
        { name: 'parameterName', type: 'string' }
    ]
});

//banner_link_mode
var linkModelStore = Ext.create("Ext.data.Store", {
    model: 'gigade.paraModel',
    autoLoad: true,
    proxy: {
        type: 'ajax',
        url: '/Parameter/QueryPara?paraType=element_link_mode',
        noCache: false,
        getMethod: function () { return 'get'; },
        actionMethods: 'post',
        reader: {
            type: 'json',
            root: 'items'
        }
    }
});
//elementTypeStore
var elementTypeStore = Ext.create("Ext.data.Store", {
    model: 'gigade.paraModel',
    autoLoad: true,
    proxy: {
        type: 'ajax',
        url: '/Parameter/QueryPara?paraType=element_type',
        noCache: false,
        getMethod: function () { return 'get'; },
        actionMethods: 'post',
        reader: {
            type: 'json',
            root: 'items'
        }
    },
    sorters: [{
        property: 'parameterCode',
        direction: 'asc'
    }]
});

//page
Ext.define("gigade.Page", {
    extend: 'Ext.data.Model',
    fields: [
        { name: "page_id", type: "string" },
        { name: "page_name", type: "string" }]
});

var PageStore = Ext.create('Ext.data.Store', {
    model: 'gigade.Page',
    autoLoad: true,
    proxy: {
        type: 'ajax',
        url: "/Element/GetPage",
        actionMethods: 'post',
        reader: {
            type: 'json',
            root: 'data'
        }

    }
});
//area
Ext.define("gigade.Area", {
    extend: 'Ext.data.Model',
    fields: [
        { name: "area_id", type: "string" },
        { name: "area_name", type: "string" },
         { name: "element_type", type: "string" }
    ]
});

var AreaStore = Ext.create('Ext.data.Store', {
    model: 'gigade.Area',
    autoLoad: true,
    proxy: {
        type: 'ajax',
        url: "/Element/GetArea",
        actionMethods: 'post',
        reader: {
            type: 'json',
            root: 'data'
        }

    }
});
//element
Ext.define("gigade.Element", {
    extend: 'Ext.data.Model',
    fields: [
        { name: "element_id", type: "string" },
        { name: "element_name", type: "string" }]
});

var ElementStore = Ext.create('Ext.data.Store', {
    model: 'gigade.Element',
    autoLoad: true,
    proxy: {
        type: 'ajax',
        url: "/Element/GetElement",
        actionMethods: 'post',
        reader: {
            type: 'json',
            root: 'data'
        }

    }
});







//前台分類store
var frontCateStore = Ext.create('Ext.data.TreeStore', {
    autoLoad: true,
    proxy: {
        type: 'ajax',
        url: '/ProductList/GetFrontCatagory',
        actionMethods: 'post'
    },
    root: {
        children: []
    }
});
frontCateStore.load();

//時間

//時間

function Tomorrow() {
    var d;
    d = new Date();                             // 创建 Date 对象。
    d.setDate(d.getDate() + 1);
    return d;
}
