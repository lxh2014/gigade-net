/*
*
* 文件摘要：XML管理前台.js
* 監    製：YellowBaby520
* 創建日期：2015.10.15
* 版權所有：GigadeTeam-WH
*
*/
///document.documentElement.clientHeight <= 700 ? document.documentElement.clientHeight - 240 : document.documentElement.clientHeight - 221,
var index_id = -1;///用來記錄當前選中點點索引序號
var data;///用來記錄所選節點
var oldNode = {}///定義一個對象用來記錄節點的原始值
var type = 0;///定義一個type用來記錄是添加什麽類型的節點操作  0:自身 1:兄弟節點 2:子節點 3:刪除自身節點 4:添加文件名


///xml對象
Ext.define('MyTreeModel', {
    extend: 'Ext.data.Model',
    fields: [
        { name: 'rowId', type: 'int' },
        { name: 'baseUrl', type: 'string' },
        { name: 'xmlStr', type: 'string' },
        { name: 'fileName', type: 'string' },
        { name: 'name', type: 'string' },
        { name: 'brotherName', type: 'string' },
        { name: 'childName', type: 'string' },
        { name: 'parentName', type: 'string' },
        { name: 'code', type: 'string' },
        { name: 'isLastNode', type: 'boolean' },
        { name: 'isTopNode', type: 'boolean' },
        { name: 'hasAttributes', type: 'boolean' },
        { name: 'expanded', type: 'boolean' },
        { name: 'text', type: 'string' },
        { name: 'attributes', type: 'string' },
        //{ name: 'checked', type: 'boolean' },
        { name: 'leaf', type: 'boolean' },
        { name: 'attr_name', type: 'string' },
        { name: 'attr_value', type: 'string' }
    ]
});

Ext.define('XMLGirdModel', {
    extend: 'Ext.data.Model',
    fields: [
        { name: 'rowId', type: 'int' },
        { name: 'fileName', type: 'string' },
        { name: 'baseUrl', type: 'string' }
    ]
});

///通過iframe用來預覽xml中的內容
function showFileName(fileName) {
    var url = window.location.host;
    url = "http://" + url + "/XmlManage/XmlFile?fileName=" + fileName;
    return "<iframe scrolling='yes' frameborder=0 width=100% height=100% src=\'" + url + "\'></iframe>";
}


//treeStore
var treeStore = Ext.create('Ext.data.TreeStore', {
    proxy: {
        type: 'ajax',
        url: '/XmlManage/XmlList',
        actionMethods: 'post'
    },
    root: {
        id: -1,
        expanded: true,
        children: [

        ]
    }
});

//treeStore.load();


///查詢按鈕的方法
function Search() {
    var xmlName = Ext.getCmp('xmlName').getValue();
    XMLGirdListStore.removeAll();
    XMLGirdListStore.load({
        params: {
            fileName: xmlName
        }
    });
}

///獲取xml文件名稱的store
var XMLGirdListStore = Ext.create('Ext.data.Store', {
    model: 'XMLGirdModel',
    proxy: {
        type: 'ajax',
        url: '/XmlManage/GetFileName',
        actionMethods: 'post',
        reader: {
            type: 'json',
            root: 'item'
        }
    }
})

XMLGirdListStore.load();

///刪除文件方法
function deleteFile(fileName) {
    Ext.Msg.confirm(CONFIRM, SUER_DELETE, function (btn) {
        if (btn == 'yes') {
            Ext.Ajax.request({
                url: '/XmlManage/DeleteFile',
                method: 'post',
                params: {
                    "fileName": fileName
                },
                success: function (response) {
                    var data = eval("(" + response.responseText + ")");
                    if (data.success) {
                        Ext.Msg.alert(MESSAGE, SUCCEED);
                        XMLGirdListStore.load();
                        treeStore.load();
                        document.getElementById('southPanel-body').innerHTML = '';
                        index_id = -1;
                        ClearParame("save");
                    }
                }
            });
        } else {
            return;
        }
    })
}

///顯示xml文件名稱的View
///設置圖片樣式與刪除xml文件
var dataview = Ext.create('Ext.view.View', {
    store: XMLGirdListStore,
    tpl: Ext.create('Ext.XTemplate',
        '<tpl for=".">',
            '<div onmouseover=onMouseoverFun(this); onmouseout=onMouseoutFun(this); class="phone">',
                '<img style="margin:-5px" width="18" height="18" onclick =deleteFile(\"{fileName}\") src="../../../Content/img/icons/cross.gif"> <span>{fileName}</span>',
            '</div>',
        '</tpl>'
    ),
    id: 'phones',
    height: document.documentElement.clientHeight <= 700 ? document.documentElement.clientHeight - 360 : document.documentElement.clientHeight - 457,
    //height: 355,
    itemSelector: 'div.phone',
    overItemCls: 'phone-hover',
    //scrollable: true,
    autoScroll: true,
    listeners: {
        itemclick: function (record, item, index, e, eOpts) {
            var selectFile = item;
            var fileName = selectFile.data.fileName;
            treeStore.load({
                params: {
                    fileName: fileName
                }
            });

            ///換文件時也需要清空信息
            ClearParame("save");

            ///獲得返回的iframe HTML
            var html = showFileName(fileName);
            /*  設置iframe的內容
                southPanel-body:該Id由southPanel中所屬的HTML標籤得到,用於顯示Panel中的內容
                根據Ext的命名原理得知:Panel的內容會放在 (Panel 的 Id + "-body")的div中    */
            document.getElementById('southPanel-body').innerHTML = html;
            $("#tree-body").css("overflow", "auto");

            //$("#tree-body").css("overflow-y", "auto");

        }
    }
});

///鼠標事件的方法
function onMouseoverFun(tagNode) {
    tagNode.style.background = "#eee";
}

///鼠標事件的方法
function onMouseoutFun(tagNode) {
    tagNode.style.background = "transparent";
}

//XMLGirdListStore.load();

///定義搜索xml文件名稱的控件
var searchForm = Ext.create('Ext.form.Panel', {
    width: 360,
    height: 50,
    frame: true,
    layout: 'column',
    items: [{
        xtype: 'textfield',
        fieldLabel: NAMEFORXML,//XMl名稱
        id: 'xmlName',
        labelWidth: 50
    }, {
        xtype: 'button',
        text: SEARCH,//查詢
        id: 'xmlSearch',
        iconCls: 'ui-icon ui-icon-search',
        margin: '0 0 0 8',
        handler: Search
    }, {
        xtype: 'button',
        text: ADD,//查詢
        id: 'xmlAdd',
        iconCls: 'ui-icon ui-icon-add',
        margin: '0 0 0 5',
        handler: Add
    }]
})

//添加XMLのwindow
var addXml = Ext.create('Ext.window.Window', {
    title: ADDXML,
    width: 300,
    layout: 'anchor',
    closeAction: 'hide',
    resizable: true,
    labelWidth: 60,
    x: 230,
    y: 100,
    items: [{
        xtype: 'textfield',
        fieldLabel: NAMEFORXML,//XMl名稱
        id: 'c_xmlName',///id與xmlName重名導致錯誤
        margin: '5 5 5 5',
        labelWidth: 78
    }, {
        xtype: 'textfield',
        fieldLabel: ROOTNAME,//XMl名稱
        id: 'rootName',
        margin: '5 5 5 5',
        labelWidth: 78
    }, {
        xtype: 'label',
        id: 'ts',
        text: TIPS,
        margin: '5 5 5 5'
    }, {
        id: 'XMLAdd',
        xtype: 'button',
        iconCls: 'ui-icon ui-icon-checked',
        text: SAVE,
        margin: '5 0 5 120',
        listeners: {
            click: function () {
                ///判斷是否為空
                if (Ext.getCmp('c_xmlName').getValue() == '') {
                    Ext.Msg.alert(INFORMATION, WRITEXMLNAME);
                    return;
                } else {
                    ///將type類型改為4 意為:刪除xml文件的操作
                    type = 4;
                    var rootName = Ext.getCmp("rootName").getValue() == "" ? "root" : Ext.getCmp("rootName").getValue();
                    if (VerifyRegex(rootName) == false || VerifyRegex(Ext.getCmp("c_xmlName").getValue()) == false) {
                        Ext.Msg.alert(INFORMATION, ROOTORNODEEXCEPTION);
                        return;
                    }
                    var fileName = Ext.getCmp("c_xmlName").getValue();
                    var result = fileName.indexOf(".xml");
                    if (result == -1) {
                        fileName = fileName + ".xml";
                    }



                    Ext.Ajax.request({
                        url: '/XmlManage/UpdateNode',
                        method: 'post',
                        params: {
                            "name": rootName,
                            "brotherName": "",
                            "childName": "",
                            "type": type,
                            "rowId": -1,
                            "code": "",
                            "attributes": "",
                            "fileName": fileName
                        },
                        success: function (response) {
                            var data = eval("(" + response.responseText + ")");
                            if (data.success) {
                                Ext.Msg.alert(MESSAGE, SUCCEED);
                                XMLGirdListStore.load();
                                addXml.hide();
                                var fileName = Ext.getCmp("c_xmlName").setValue("");
                                var rootName = Ext.getCmp("rootName").setValue("");
                            }
                        }
                    });
                }
            }
        }
    }]
})


function Add() {
    addXml.show();
}

///節點詳細資料store
var XMLItemStore = Ext.create('Ext.data.Store', {
    model: 'MyTreeModel',
    proxy: {
        type: 'ajax',
        url: '',
        actionMethods: 'post',
        reader: {
            type: 'json'
        }
    }
})

///中間節點panel
var tree = Ext.create('Ext.tree.Panel', {
    id: 'tree',
    width: 300,
    height: document.documentElement.clientHeight <= 700 ? document.documentElement.clientHeight - 339 : document.documentElement.clientHeight - 417,
    //height: 382,
    frame: false,
    lines: true,
    layout: 'anchor',
    rootVisible: false,
    onlyLeafCheckable: true,
    lines: true,
    enableDrag: true,
    autoScroll: true,
    store: treeStore,
    listeners: {
        itemclick: function (record, item, index, e, eOpts) {
            ///設置type = 0;
            type = 0;

            ///設置添加兄弟,子節點 兩按鈕的顏色,選中為紅色,未選中為黑色
            Ext.getCmp('brother').btnInnerEl.setStyle('color', "black");
            Ext.getCmp('child').btnInnerEl.setStyle('color', "black");

            $("#XMLItemGird-body").children().first().css("overflow", "auto");
            ///清空節點詳細的store
            XMLItemStore.removeAll();
            /// 保存節點資料
            oldNode = data = item.raw;
            ///獲得節點索引
            index_id = e;

            ///設置節點名稱
            Ext.getCmp('name').setValue(data.name);

            ///設置節點內容
            Ext.getCmp('code').setValue(data.code);


            ///設置節點詳細的store
            if (data.attributes == "") {
                return;
            }

            ///用'|'分割
            var attribute = data.attributes.split('|');

            for (var i = 0; i < attribute.length; i++) {
                if (attribute[i] != "") {
                    var attr = attribute[i].split('=');
                    var attr_name = attr[0];
                    var attr_value = attr[1];
                    XMLItemStore.add({
                        rowId: index_id,
                        name: data.name,
                        code: data.code,
                        attr_name: attr_name,
                        attr_value: attr_value
                    });
                }
            }
        }
    }
});





//XMLItemGird
var XMLItemGird = Ext.create('Ext.grid.Panel', {
    plugins: [{ ptype: 'cellediting' }],
    store: XMLItemStore,
    resizable: true,
    id: 'XMLItemGird',
    flex: 3.0,
    autoScroll: true,
    height: document.documentElement.clientHeight <= 700 ? document.documentElement.clientHeight - 340 : document.documentElement.clientHeight - 420,
    //height: 200,
    columns: [{ xtype: 'rownumberer', width: 25, align: 'center' },
              {
                  text: '', menuDisabled: true, width: 35, align: 'center', xtype: 'actioncolumn',
                  icon: '../../../Content/img/icons/cross.gif',
                  handler: function (grid, rowIndex, colIndex) {
                      XMLItemStore.removeAt(rowIndex);
                  }
              }, {
                  header: CHOOSEID, dataIndex: 'rowId', width: 80, align: 'center', menuDisabled: true, sortable: false
              }, {
                  header: PROPERTYNAME, dataIndex: 'attr_name', width: 280, align: 'center', menuDisabled: true, sortable: false, editor: { xtype: 'textfield' }
              }, {
                  header: PROPERTYVALUE, dataIndex: 'attr_value', width: 280, align: 'center', menuDisabled: true, sortable: false, editor: { xtype: 'textfield' }
              }, {
                  header: '', width: 270, align: 'center', menuDisabled: true, sortable: false
              }],
    tbar: [{
        xtype: 'textfield',
        id: 'name',
        fieldLabel: POINTNAME,
        allowBlank: true,///設置為允許為空,因為保存方法中已經對其作出為空判斷
        labelWidth: 60
    }, {
        xtype: 'textfield',
        id: 'code',
        fieldLabel: POINTCONTENT,
        allowBlank: true,
        labelWidth: 60
    }],
    dockedItems: [{
        id: 'dockedItem',
        xtype: 'toolbar',
        layout: 'column',
        dock: 'top',
        items: [{
            text: ADDPROPERTY, handler: function (grid, rowIndex, colIndex) {
                if (index_id == -1) {
                    Ext.Msg.alert(INFORMATION, P_C_ROOT);
                    return;
                }
                XMLItemStore.add(
                    {
                        rowId: index_id,
                        //name: data.name,
                        //scode: data.code,
                        attr_name: "",
                        attr_value: ""
                    }
                );
            }
        }, '-', {
            text: ADD_BROOT, id: 'brother', handler: function () {
                type = 1;
                Ext.getCmp('brother').btnInnerEl.setStyle('color', "red");
                Ext.getCmp('child').btnInnerEl.setStyle('color', "black");
                ClearParame();
            }

        }, '-', {
            text: ADD_CROOT, id: 'child', handler: function () {
                type = 2;
                Ext.getCmp('child').btnInnerEl.setStyle('color', "red");
                Ext.getCmp('brother').btnInnerEl.setStyle('color', "black");
                ClearParame();
            }
        }, '-', {
            text: DELETE_ROOT, handler: function () {
                Ext.Msg.confirm(CONFIRM, SUER_DELETE, function (btn) {
                    if (btn == 'yes') {
                        type = 3;
                        Ext.getCmp('brother').btnInnerEl.setStyle('color', "black");
                        Ext.getCmp('child').btnInnerEl.setStyle('color', "black");
                        Save();
                    } else {
                        return;
                    }
                })
            }
        }]
    }, {
        xtype: 'toolbar',
        layout: 'column',
        dock: 'bottom',
        items: [{
            id: 'XMLUpdate',
            xtype: 'button',
            iconCls: 'ui-icon ui-icon-checked',
            text: SAVE,
            listeners: {
                click: function () {
                    Save();
                }
            }
        }]
    }]

})


//var CenteRegio = ('Ext.container.Container', {
//    id: 'CenteRegio',
//    //autoScroll: true,
//    frame: false,
//    border: 0,
//    layout: { type: 'hbox' },
//    defaults: { margin: '3 3 0 3' },
//    items: [tree, XMLItemGird]
//})

var westRegio = ('Ext.container.Container', {
    id: 'westRegio',
    //autoScroll: true,
    frame: false,
    //border: 0,
    layout: 'anchor',
    defaults: { margin: '3 3 0 3' },
    items: [searchForm, dataview]
})



Ext.create('Ext.Viewport', {
    id: 'xmlIndex',
    layout: 'border',
    //autoScroll: true,
    items: [{
        id: 'west-region',
        region: 'west',
        xtype: 'panel',
        //collapsible: true,
        //autoScroll: true,
        //height:300,
        frame: false,
        width: 360,
        layout: 'anchor',
        //split: true,
        //margin: '0 3 0 0',
        items: westRegio
    }, {
        id: 'center-region',
        region: 'center',
        xtype: 'panel',
        frame: false,
        //autoScroll: true,
        layout: { type: 'hbox' },
        //spilt:true,
        items: [tree, XMLItemGird]
    }, {
        region: 'south',
        xtype: 'panel',
        id: 'southPanel',
        flex: 1,
        //height: 400,
        layout: 'fit',
        autoScroll: false,
        split: true
    }],
    listeners: {
        resize: function () {
            Ext.getCmp("xmlIndex").width = document.documentElement.clientWidth;
            Ext.getCmp("xmlIndex").height = document.documentElement.clientHeight;
            Ext.getCmp('center-region').setAutoScroll = true;
            Ext.getCmp('west-region').setAutoScroll = true;
            this.doLayout();
        }
    }
});

function Save() {
    var paramer = {}
    var msg = VerifyParamer();
    ///index_id等於-1 表示未選中節點
    if (index_id == -1) {
        Ext.Msg.alert(INFORMATION, P_C_ROOT);
        return;
    }

    ///type==2時表示根節點添加子節點,該項操作時允許的
    if (index_id == 0 && type != 2) {
        Ext.Msg.alert(INFORMATION, DONTDOFORPROOT);
        return;
    }
    if (msg != "success") {
        Ext.Msg.alert(INFORMATION, msg);
        return;
    }

    if (!VerifyRegex(Ext.getCmp('name').getValue())) {
        Ext.Msg.alert(INFORMATION, DONTNAMEINNUMSTART);
        return;
    }
    var nodeName = Ext.getCmp('name').getValue();
    paramer.type = type;
    paramer.fileName = oldNode.fileName;
    paramer.rowId = index_id;
    paramer.code = Ext.getCmp('code').getValue();
    switch (type) {
        case 0:
            if (nodeName == "") {
                paramer.name = oldNode.name;
            } else {
                paramer.name = nodeName;
            }
            paramer.brotherName = "";
            paramer.childName = "";
            break;
        case 1:
            paramer.name = oldNode.name;
            paramer.brotherName = nodeName;
            paramer.childName = "";
            break;
        case 2:
            paramer.name = oldNode.name;
            paramer.brotherName = "";
            paramer.childName = nodeName;
            break;
        case 3:
            paramer.name = oldNode.name;
            paramer.brotherName = "";
            paramer.childName = nodeName;
            break;
    }

    var attrStr = "";
    var attribute = XMLItemStore.data.items;
    for (var i = 0; i < attribute.length; i++) {
        if (!VerifyRegex(attribute[i].data.attr_name)) {
            Ext.Msg.alert(INFORMATION, DONTNAMEINNUMSTART);
            return;
        }
        var tempStr = attribute[i].data.attr_name + "=" + attribute[i].data.attr_value
        attrStr += tempStr + "|";
    }
    paramer.attributes = attrStr;
    Ext.Ajax.request({
        url: '/XmlManage/UpdateNode',
        method: 'post',
        params: {
            "name": paramer.name,
            "brotherName": paramer.brotherName,
            "childName": paramer.childName,
            "type": paramer.type,
            "rowId": paramer.rowId,
            "code": paramer.code,
            "attributes": paramer.attributes,
            "fileName": paramer.fileName
        },
        success: function (response) {
            var data = eval("(" + response.responseText + ")");
            if (data.success) {
                Ext.Msg.alert(INFORMATION, SUCCEED);
                ///獲得返回的iframe HTML
                var html = showFileName(oldNode.fileName);
                ///設置iframe的內容
                /*
                    southPanel-body:該Id由southPanel中所屬的HTML標籤得到,用於顯示Panel中的內容
                    根據Ext的命名原理得知:Panel的內容會放在 (Panel 的 Id + "-body")的div中
                */
                document.getElementById('southPanel-body').innerHTML = html;

                /*
                    原理同上
                */
                $("#tree-body").css("overflow", "auto");
                treeStore.load({
                    params: {
                        fileName: oldNode.fileName
                    }
                });

                ClearParame("save");
            }
        }
    });

}

function VerifyParamer() {
    var error = "success";
    if (Ext.getCmp('name').getValue() == "" && type != 0) {
        error = ROOTNAMEDONTBLACK;
    }
    return error;
}

///清空參數
function ClearParame(deleteType) {
    XMLItemStore.removeAll();
    Ext.getCmp("name").setValue("");
    Ext.getCmp("code").setValue("");
    ///如果是保存成功后調用該方法,則連index_id也初始化
    if (deleteType == "save") {
        index_id = -1;
    }
}


///只能輸入數字的驗證
function VerifyRegex(testValue) {
    //var regex = /^[A-Za-z]+$/;
    //var regex = /^[a-zA-Z\u0391-\uFFE5].{15}$/;

    //var regex0 = /^\w+$/;
    var regex1 = /^[A-Za-z]\w+$/;
    var regex2 = /^[A-Za-z]$/;
    //var regex = /^[A-Za-z]$/;
    //var regex1 = /^[A-Za-z]+[A-Za-z]$/;
    //var regex2 = /^[A-za-z][0-9a-zA-z]+[A-Za-z]$/;
    if (regex2.test(testValue)) {
        return true;
    } else {
        if (regex1.test(testValue)) {
            return true;
        }
    }
    //else if (regex1.test(testValue)) {
    //    return true;
    //}else if (regex2.test(testValue)) {
    //    return true;
    //}
    return false;
}