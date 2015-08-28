
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
            if (menuRoots == "") {
                Ext.MessageBox.confirm(CONFIRM, NO_AUTH, function (btn) {
                    if (btn == "yes") {
                        window.opener = null;
                        window.open('', '_self');
                        window.close();
                    }
                    else {
                        return false;
                    }
                });

            } else {
                createForm();
            }
        }
    })
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

    //中間panel
    var center = Ext.create('Ext.tab.Panel', {
        id: 'ContentPanel',
        region: 'center',
        margins: '0 5 0 0',
        title: '<div align="right" style="margin-right: 20px">' + WELCOME + '：<font color="red"  style="margin-right: 20px">' + document.getElementById("lgnName").value + '</font><a href="#" style="margin-right: 10px;display:none">' + EDIT_INFO + '</a><a href="javascript:exit()" >' + LOGOUT + '</a></div>',
        activeTab: 0,
        items: []
    });
    //頭部
    var top = Ext.create('Ext.panel.Panel', {
        region: 'north',
        height: 100,
        title: '',
        html: rtnFrame('Home/Top')
    });
    //底部
    var down = Ext.create('Ext.panel.Panel', {
        id: 'downPanel',
        region: 'south',
        border: 0,
        tbar: [
                { xtype: 'tbtext', text: CURRENT_TIME + ':', style: { paddingLeft: '85%' } },
                { xtype: 'tbtext', id: 'toolClock', text: Ext.Date.format(new Date(), 'Y/m/d H:i:s') },
                { xtype: 'tbspacer' }/*,
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