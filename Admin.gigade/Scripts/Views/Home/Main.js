
var menuRoots = new Array();
var menuTree = new Array();
var menuStore = new Array();

Ext.define('GIGADE.Function', {
    extend: 'Ext.data.Model',
    fields: [
                { name: "text", type: "string" },
                { name: "url", type: "string" },
                { name: "id", type: "string" },
                { name: "leaf", type: "boolean" },
                { name: "cls", type: "string" }
    ]
});

Ext.onReady(function () {
    Ext.Ajax.request({
        url: '/FunctionGroup/GetAuthorityGroup',
        method: 'post',
        success: function (response) {
            menuRoots = eval(response.responseText);
            createForm();
        }
    });
});

function createForm() {

    createTree();

    //左邊panel
    var west = Ext.create('Ext.panel.Panel', {
        region: 'west',
        title: '',
        split: true,
        width: 250,
        margins: '0 0 0 5',
        collapsible: true,
        layout: 'accordion',
        layoutConfig: {
            animate: true
        },
        items: menuTree
    });

    //style="margin-right: 20px" width="16" height="16"
    //中間panel
    var center = Ext.create('Ext.tab.Panel', {
        id: 'ContentPanel',
        region: 'center',
        margins: '0 5 0 0',
        title: '<div align="right" style="margin-right: 20px"><img src="../../../Content/img/icons/heart.png" width="16" height="16" style="margin: 0px 5px 0px 0px; vertical-align:bottom;"/>' + WELCOME + '：<font color="red"  style="margin-right: 20px">' + document.getElementById("lgnName").value + '</font><a href="#" style="margin-right: 10px;display:none">' + EDIT_INFO + '</a><a href="javascript:exit()" ><img src="../../../Content/img/icons/door_out.png" style="margin: 0px 5px 0px 0px; vertical-align:bottom;"/>' + LOGOUT + '</a></div>',
        activeTab: 0,
        items: []
    });
    //頭部
    var top = Ext.create('Ext.panel.Panel', {
        region: 'north',
        height: 100,
        border: 0,
        title: '',
        html: rtnFrame('Home/Top')
    });
    //底部
    var down = Ext.create('Ext.panel.Panel', {
        id: 'downPanel',
        region: 'south',
        border: 0,
        tbar: [
                { xtype: 'tbtext', text: CURRENT_TIME + ':', style: { paddingLeft: '85%'} },
                { xtype: 'tbtext', id: 'toolClock', text: Ext.Date.format(new Date(), 'Y/m/d H:i:s') },
                { xtype: 'tbspacer'}/*,
                { xtype: 'tbtext', text: '在线人数:' },
                { xtype: 'tbtext', text: 1, padding: '0 0 0 10' }*/
        ]
    });
    Ext.create('Ext.Viewport', {
        layout: 'border',
        items: [
            top,
            west,
            down,
            center
        ]
    })
}


//得到權限菜單
function createTree() {
    for (var i = 0; i < menuRoots.length; i++) {
        menuStore[i] = Ext.create('Ext.data.TreeStore', {
            proxy: {
                type: 'ajax',
                url: '/FunctionGroup/GetAuthorityFun/' + menuRoots[i].Id,
                actionMethods: 'post'
            },
            root: {
                id: menuRoots[i].Id,
                expanded: true,
                text: ''
            },
            model: 'GIGADE.Function'
        });

        menuTree[i] = Ext.create('Ext.tree.Panel', {
            autoScroll: true,
            title: menuRoots[i].Text,
            collapsible: true,
            width: 200,
            useArrows: true,
            store: menuStore[i],
            rootVisible: false,
            listeners: {
                itemclick: function (view, record, item, index, e, eOpts) {
                    if (record.data.leaf) {
                        RememberHistory(view, record, item, index, e, eOpts); //add by wwei0216w 用於記錄用戶的操作記錄 2015/4/3
                        var tab = Ext.getCmp(record.data.id);
                        var center = Ext.getCmp('ContentPanel');
                        if (!tab) {
                            tab = center.add({
                                title: record.data.text,
                                id: record.data.id,
                                html: rtnFrame(record.raw.url),
                                closable: true
                            });
                        }
                        center.setActiveTab(tab);
                        center.doLayout();
                    }
                }
            }
        });
    }
}

function rtnFrame(url) {
    return "<iframe scrolling='no' frameborder=0 width=100% height=100% src='" + url + "'></iframe>";
}

function exit() {
    window.top.location.replace("http://" + window.parent.location.host + "/Login/Logout"); //document.getElementById("lgnEmail").value
}

//當前時間
setInterval(function () {
    setClock()
}, 1000);
function setClock() {
    var date = new Date();
    var clock = Ext.getCmp('toolClock');
    var curTime = Ext.Date.format(new Date(), 'Y/m/d H:i:s');
    clock.setText(curTime);
}

//add by wwei0216w 2015/04/3
///用於記錄點點擊該按鈕的一些事件信息
function RememberHistory(view, record, item, index, e, eOpts) {
    var function_id = record.data.id;
    Ext.Ajax.request({
        url: '/FunctionGroup/RememberHistory',
        method: 'POST',
        params: {
            "function_id": function_id
        },
        success: function (msg) {
            var resMsg = eval("(" + msg.responseText + ")");
            if (resMsg.success == true) {
                return true;
            } else {
                return false;
            }
        }
    });
}