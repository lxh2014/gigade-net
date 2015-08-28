function Today() {
    var d;
    var s = "";
    d = new Date();                             // 创建 Date 对象。
    s += d.getFullYear() + "/";                     // 获取年份。
    s += (d.getMonth() + 1) + "/";              // 获取月份。
    s += d.getDate();                          // 获取日。
    return (new Date(s));                                 // 返回日期。
};
//上個月
function Tomorrow() {
    var d;
    var s = "";
    d = new Date();                             // 创建 Date 对象。
    s += d.getFullYear() + "/";                 // 获取年份。
    s += d.getMonth() + "/";              // 获取月份。
    s += d.getDate();                       // 获取日。
    return (new Date(s));                       // 返回日期。
}
//數字千分位
function change(value) {
    value = value.toString();
    if (/^\d+$/.test(value)) {
        value = value.replace(/^(\d+)(\d{3})$/, "$1,$2");
    }
    return value;
}
//供應商Model
Ext.define("gigade.Vendor", {
    extend: 'Ext.data.Model',
    fields: [
        { name: "vendor_id", type: "string" },
        { name: "vendor_name_simple", type: "string" }]
});
var VendorStore = Ext.create('Ext.data.Store', {
    model: 'gigade.Vendor',
    autoDestroy: true,
    proxy: {
        type: 'ajax',
        url: "/OrderManage/GetVendor",
        actionMethods: 'post',
        reader: {
            type: 'json',
            root: 'data'
        }
    }
});

/*品牌列表:Driver 環保創意生活家*/
Ext.define('gigade.VendorBrand', {
    extend: 'Ext.data.Model',
    fields: [
        { name: "Brand_Id", type: "string" },
        { name: "Brand_Name", type: "string" }
    ]
});
var VendorBrandStore = Ext.create('Ext.data.Store', {
    autoDestroy: true,
    model: 'gigade.VendorBrand',
    proxy: {
        type: 'ajax',
        url: '/OrderManage/GetVendorBand',
        actionMethods: 'post',
        reader: {
            type: 'json',
            root: 'data'
        }
    }
});

//減免活動Model
Ext.define('gigade.PromotionsAmoutReduce', {
    extend: 'Ext.data.Model',
    fields: [
        { name:'id', type:'int' },
        { name:'name', type:'string' }
    ]
});
var PromotionsAmoutReduceStore = Ext.create('Ext.data.Store', {
    model: 'gigade.PromotionsAmoutReduce',
    autoDestroy: true,
    proxy: {
        type: 'ajax',
        url: "/OrderManage/GetPromotionsAmoutReduce",
        actionMethods: 'post',
        reader: {
            type: 'json',
            root: 'data'
        }
    }
});

//會員群組Model
Ext.define('gigade.VipUserGroup', {
    extend: 'Ext.data.Model',
    fields: [
        { name: 'group_id', type: 'uint' },
        { name: 'group_name', type: 'string' }
    ]
});
var VipUserGroupStore = Ext.create('Ext.data.Store', {
    model: 'gigade.VipUserGroup',
    autoDestroy: true,
    proxy: {
        type: 'ajax',
        url: "/OrderManage/GetVipUserGroup",
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
        { name: 'ParameterCode', type: 'string' },
        { name: 'parameterName', type: 'string' }
    ]
});
var paymentType = Ext.create('Ext.data.Store', {
    autoDestroy: true,
    model: 'gigade.paraModel',
    proxy: {
        type: 'ajax',
        url: '/OrderManage/GetPayMentType?paraType=payment',
        actionMethods: 'post',
        reader: {
            type: 'json',
            root: 'data'
        }
    }
});
//運送方式
var FreightSetStore = Ext.create('Ext.data.Store', {
    autoDestroy: true,
    model: 'gigade.paraModel',
    proxy: {
        type: 'ajax',
        url: '/OrderManage/GetPayMentType?paraType=product_freight',
        actionMethods: 'post',
        reader: {
            type: 'json',
            root: 'data'
        }
    }
});
//訂單狀態
Ext.define('gigade.PaymentTypes', {
    extend: 'Ext.data.Model',
    fields: [
        { name: "ParameterCode", type: "string" },
        { name: "remark", type: "string" }
    ]
});
var paymentStore = Ext.create('Ext.data.Store', {
    autoDestroy: true,
    model: 'gigade.PaymentTypes',
    proxy: {
        type: 'ajax',
        url: '/OrderManage/GetPayMentStatus?paraType=order_status',
        actionMethods: 'post',
        reader: {
            type: 'json',
            root: 'data'
        }
    }
});

//類別Model
Ext.define("gigade.Category", {
    extend: 'Ext.data.Model',
    fields: [
        { name: "category_id", type: "string" },
        { name: "category_name", type: "string" }]
});
var CateStore = Ext.create('Ext.data.Store', {
    model: 'gigade.Category',
    //autoLoad: true,//頁面自動加載
    proxy: {
        type: 'ajax',
        url: "/OrderManage/GetCategory",
        actionMethods: 'post',
        reader: {
            type: 'json',
            root: 'data'
        }
    }
});


/*賣場，chanel,幾家地，雅虎商城*/
Ext.define('gigade.Channel', {
    extend: 'Ext.data.Model',
    fields: [
        { name: "channel_id", type: "string" },
        { name: "channel_name_simple", type: "string" }
    ]
});
var ChannelStore = Ext.create('Ext.data.Store', {
    autoDestroy: true,
    model: 'gigade.Channel',
    proxy: {
        type: 'ajax',
        url: '/OrderManage/GetChannel',
        actionMethods: 'post',
        reader: {
            type: 'json',
            root: 'data'
        }
    }
});

/*管理人員列表：卓靜怡，莊珮君 */
Ext.define('gigade.ManageUser', {
    extend: 'Ext.data.Model',
    fields: [
        { name: "userId", type: "string" },
        { name: "userName", type: "string" }
    ]
});
var ProductManageStore = Ext.create('Ext.data.Store', {
    autoDestroy: true,
    model: 'gigade.ManageUser',
    proxy: {
        type: 'ajax',
        url: '/Vendor/QueryPm',
        actionMethods: 'post',
        reader: {
            type: 'json',
            root: 'item'
        }
    }
});
