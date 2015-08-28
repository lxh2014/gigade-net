//物流商【營管>出貨查詢】

Ext.define("gigade.paraModel", {
    extend: 'Ext.data.Model',
    fields: [
        { name: 'parameterCode', type: 'string' },
        { name: 'parameterName', type: 'string' }
    ]
});

var DeliverStore= Ext.create("Ext.data.Store", {
    model: 'gigade.paraModel',
            autoLoad: true,
        proxy: {
        type: 'ajax',
        url: '/SendProduct/QueryPara?paraType=Deliver_Store',
                //noCache: false,
                //getMethod: function () { return 'get'; },
                //        timeout:180000,
            actionMethods: 'post',
                reader: {
                type: 'json',
                        root: 'items'
        }
        }
});

//出貨時間
Ext.define("gigade.sendProTime", {
    extend: 'Ext.data.Model',
    fields: [
        {name:'day',type:'string'},
        {name:'time',type:'string'}
    ]
});

var sendProTStore = Ext.create("Ext.data.Store", {
    model: 'gigade.sendProTime',
    autoLoad: true,
    proxy: {
        type: 'ajax',
        url: '/SendProduct/SendProTime',
        noCache: false,
        getMethod: function () { return 'get'; },
        timeout: 180000,
        actionMethods: 'post',
        reader: {
            type: 'json',
            root: 'items'
        }

    }

});

function Today() {
    var d;
    var s = "";
    d = new Date();                             // 创建 Date 对象。
    var year = d.getFullYear();
    var month = d.getMonth() + 1;
    if (month < 10) {
        month = "0" + month;
    }
    var day = d.getDate();
    if (day < 10) {
        day = "0" + day;
    }
    s = year + "/" + month + "/" + day;
    return s.toString();                                 // 返回日期。
}