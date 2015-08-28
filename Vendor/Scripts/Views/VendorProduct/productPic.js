var PIC_MAX = 20;
var Spec_Id;
var errorMsg = PIC_LIMIT;
var win;
var PRODUCT_ID = '', OLD_PRODUCT_ID = '';
PRODUCT_ID = window.parent.GetProductId(); //獲取新增或修改的product_id 
OLD_PRODUCT_ID = window.parent.GetCopyProductId(); //獲取複製的product_id 
//產品規格圖model 
Ext.define("picture", {
    extend: 'Ext.data.Model',
    fields: [
{ name: "img", type: "string" },
{ name: "spec_id", type: "string" },
{ name: "spec_sort", type: "string" },
{ name: "spec_status", type: "string"}]
});

//商品說明圖model 
Ext.define("explain", {
    extend: 'Ext.data.Model',
    fields: [
{ name: "img", type: "string" },
{ name: "image_sort", type: "string" },
{ name: 'image_state', type: "string" }
]
});

//商品說明圖Store 
var explainStore = Ext.create('Ext.data.Store', {
    model: 'explain',
    proxy: {
        type: 'ajax',
        url: '/VendorProduct/QueryExplainPic',
        actionMethods: 'post',
        reader: {
            type: 'json',
            root: 'items'
        }
    }
});

//產品規格圖store 
var StandardStore = Ext.create('Ext.data.Store', {
    model: 'picture',
    proxy: {
        type: 'ajax',
        url: '/VendorProduct/QuerySpecPic',
        actionMethods: 'post',
        reader: {
            type: 'json',
            root: 'items'
        }
    }
});

StandardStore.on('beforeload', function () {
    Ext.apply(StandardStore.proxy.extraParams,
{
    product_id: Ext.htmlEncode(window.parent.GetProductId()),
    OldProductId: window.parent.GetCopyProductId()
});
});

explainStore.on('beforeload', function () {
    Ext.apply(explainStore.proxy.extraParams,
{
    product_id: Ext.htmlEncode(window.parent.GetProductId()),
    OldProductId: window.parent.GetCopyProductId()
});
});


//規格一store 
var SpecStore = Ext.create("Ext.data.Store", {
    fields: ['spec_name', 'spec_id'],
    proxy: {
        type: 'ajax',
        url: "/VendorProduct/spec1TempQuery",
        actionMethods: 'post',
        reader: {
            type: 'json',
            root: 'data'
        }
    }
});

SpecStore.on('beforeload', function () {
    Ext.apply(SpecStore.proxy.extraParams,
{
    ProductId: Ext.htmlEncode(window.parent.GetProductId()),
    OldProductId: window.parent.GetCopyProductId()
});
});


var cellEditing = Ext.create('Ext.grid.plugin.CellEditing', {
    clicksToEdit: 1
});

var cellEditingEx = Ext.create('Ext.grid.plugin.CellEditing', {
    clicksToEdit: 1
});

var Pic


Ext.onReady(function () {
    Ext.QuickTips.init();
    SpecStore.load();
    StandardStore.load();
    Pic = Ext.create('Ext.Img', {
        width: 150,
        height: 150,
        style: {
            margin: '10 0 0 0',
            border: 'solid 1px #EEE',
            padding: '3 3 3 3'
        }
    });


    //商品說明圖 
    var expalinPic = Ext.create("Ext.grid.Panel", {
        plugins: [cellEditingEx],
        y: 5,
        id: 'explainPanel',
        store: explainStore,
        width: 700,
        height: 230,
        border: true,
        columns: [{
            xtype: 'actioncolumn',
            width: 100,
            id: 'deleteEx',
            colName: 'deleteEx',
            // hidden: true, 
            align: 'center',
            items: [{
                icon: '../../../Content/img/icons/cross.gif',
                tooltip: DELETE,
                handler: function (grid, rowIndex, colIndex) {
                    if (!confirm(SURE_TO_DELETE)) {
                        return;
                    }
                    var rec = explainStore.getAt(rowIndex).data;
                    explainStore.removeAt(rowIndex);
                    Ext.Ajax.request({
                        url: '/VendorProduct/DeletePic',
                        method: 'post',
                        params: {
                            "type": 'explain',
                            "rec": rec,
                            "product_id": ''
                        }
                    });
                }
            }]
        }, {
            header: PRODUCT_EXPLAIN_PIC,
            sortable: false,
            menuDisabled: true,
            dataIndex: 'image_filename',
            // hidden: true, 
            id: 'image_filename',
            colName: 'image_filename',
            xtype: 'templatecolumn',
            tpl: '<img width=50 name="tplImg" onmousemove="javascript:imgFadeBig(this.src,250,250);" onmouseout = "javascript:$(\'#imgTip\').hide()" height=50 src="{img}" />',
            width: 200,
            align: "center"
        }, {
            header: PIC_SORT,
            sortable: false,
            menuDisabled: true,
            width: 190,
            dataIndex: 'image_sort',
            // hidden: true, 
            id: 'image_sort',
            colName: 'image_sort',
            align: 'center',
            editor: {
                xtype: 'textfield'
            }
        }, {
            header: PIC_SHOW,
            sortable: false,
            menuDisabled: true,
            width: 200,
            dataIndex: 'image_state',
            // hidden: true, 
            id: 'image_state',
            colName: 'image_state',
            align: 'center',
            renderer: function (value) {
                if (value == "1" || value == 'true') {
                    return PIC_SHOW;
                }
                else {
                    return PIC_NOT_SHOW;
                }
            },
            editor: {
                xtype: 'checkboxfield',
                width: 40,
                labelWidth: 30
            }
        }],
        listeners: {
            scrollershow: function (scroller) {
                if (scroller && scroller.scrollEl) {
                    scroller.clearManagedListeners();
                    scroller.mon(scroller.scrollEl, 'scroll', scroller.onElScroll, scroller);
                }
            }
        }
    });
    //規格圖 
    var standard = Ext.create("Ext.grid.Panel", {
        plugins: [cellEditing],
        id: 'standardPanel',
        store: StandardStore,
        height: 180,
        title: SPEC_IMG,
        width: 700,
        border: true,
        columns: [{
            xtype: 'actioncolumn',
            width: 100,
            align: 'center',
            id: 'deleteSpec',
            colName: 'deleteSpec',
            // hidden: true, 
            items: [{
                icon: '../../../Content/img/icons/cross.gif',
                handler: function (grid, rowIndex, colIndex) {
                    if (!confirm(SURE_TO_DELETE)) {
                        return;
                    }
                    var rec = Ext.getCmp("standardPanel").store.data.items[rowIndex].data;

                    Ext.Ajax.request({
                        url: '/VendorProduct/DeletePic',
                        method: 'post',
                        params: {
                            "type": 'spec',
                            "rec": rec,
                            "product_id": Ext.htmlEncode(window.parent.GetProductId()),
                            OldProductId: window.parent.GetCopyProductId()
                        },
                        success: function (response) {
                            var data = eval("(" + response.responseText + ")");
                            var resStr = data.msg;
                            Ext.Msg.alert(PROMPT, resStr);
                            if (data.success) {
                                Ext.getCmp("standardPanel").store.data.items[rowIndex].set('img', data.path);
                            }
                        }
                    });
                }

            }]
        }, {
            header: PRODUCT_SPEC_PIC,
            sortable: false,
            menuDisabled: true,
            dataIndex: 'spec_image',
            // hidden: true, 
            id: 'spec_image',
            colName: 'spec_image',
            xtype: 'templatecolumn',
            tpl: '<img width=50 height=50 onclick = "javascript:addPic(\'spec\',{spec_id})" onmousemove="javascript:imgFadeBig(this.src,200,200);" onmouseout = "javascript:$(\'#imgTip\').hide()" src="{img}" />',
            width: 200,
            align: "center"
        }, {
            header: SPEC_ONE,
            sortable: false,
            menuDisabled: true,
            width: 130,
            dataIndex: 'spec_id',
            // hidden: true, 
            id: 'spec_id',
            colName: 'spec_id',
            name: 'spec_name',
            align: 'center',
            renderer: function (value) {
                var index = SpecStore.find("spec_id", value);
                var recode = SpecStore.getAt(index);
                if (recode) {
                    return recode.get("spec_name");
                }
            }
        }, {
            header: PIC_SORT,
            sortable: false,
            menuDisabled: true,
            width: 130,
            colName: 'spec1_sort',
            dataIndex: 'spec_sort',
            id: 'spec1_sort',
            // hidden: true, 
            align: 'center',
            editor: {
                xtype: 'textfield'
            }
        }, {
            header: PIC_SHOW,
            sortable: false,
            menuDisabled: true,
            width: 130,
            dataIndex: 'spec_status',
            // hidden: true, 
            id: 'spec1_status',
            colName: 'spec1_status',
            align: 'center',
            renderer: function (value) {
                if (value == "1" || value == 'true') {
                    return PIC_SHOW;
                }
                else {
                    return PIC_NOT_SHOW;
                }
            },
            editor: {
                xtype: 'checkboxfield',
                width: 40,
                labelWidth: 30
            }
        }],
        listeners: {
            scrollershow: function (scroller) {
                if (scroller && scroller.scrollEl) {
                    scroller.clearManagedListeners();
                    scroller.mon(scroller.scrollEl, 'scroll', scroller.onElScroll, scroller);
                }
            }
        }
    });

    var picInfo = Ext.create("Ext.panel.Panel", {
        defaults: {
            labelWidth: 80,
            padding: '5 0 0 5'
        },
        width: 1100,
        border: false,
        items: [{
            xtype: 'panel',
            style: {
                padding: '0 0 10 0'
            },
            layout: 'hbox',
            border: false,
            id: 'product_image',
            colName: 'product_image',
            // hidden: true, 
            items: [Pic, {
                xtype: 'panel',
                width: 900,
                height: 150,
                border: false,
                layout: 'absolute',
                items: [{
                    xtype: 'label',
                    text: PRODUCT_PIC,
                    x: 10,
                    y: 70
                }, {
                    xtype: 'textfield',
                    id: 'fileName',
                    width: 280,
                    readOnly: true,
                    x: 60,
                    y: 70
                }, {
                    xtype: 'button',
                    text: SELECT_IMG,
                    x: 360,
                    y: 70,
                    handler: function () {
                        addPic("prod", 0);
                    }
                }, {
                    xtype: 'label',
                    html: "<span style='font-size:10; color:Gray'>" + MESSAGE + "</span>",
                    x: 10,
                    y: 100
                }]
            }]
        }, {
            xtype: 'panel',
            width: 900,
            border: false,
            colName: 'product_media',
            // hidden: true, 
            layout: 'hbox',
            items: [{
                xtype: 'textfield',
                id: 'product_media',
                fieldLabel: '商品影片',
                labelWidth: 70,
                border: false,
                vtype: 'url',
                width: 480
            }, {
                xtype: 'displayfield',
                id: 'dis',
                value: '<span style="color:gray">*</span><a id="yTE" href="#" >YouTube嵌入程序碼取得說明</a>',
                width: 200,
                listeners: {
                    afterrender: function () {
                        Ext.create("Ext.tip.ToolTip", {
                            target: "dis",
                            maxWidth: 500,
                            width: 460,
                            height: 250,
                            html: '<img src="../../Content/img/youtube_url_link_step.jpg"/>'
                        });
                    }
                }
            }]
        }, standard, {
            xtype: 'button',
            text: ADD_EXPLAIN_PIC,
            id: 'ExpUpload',
            colName: 'ExpUpload',
            // hidden: true, 
            x: 5,
            y: 5,
            iconCls: 'icon-add',
            handler: function () { addPic("desc", 0); }
        }, expalinPic
, {
    xtype: 'button',
    text: '保存',
    hidden: true,
    iconCls: 'icon_rewind',
    handler: function () {
        SaveTo();
    },
    width: 100,
    height: 45,
    style: {
        marginLeft: '200px'
    }
}],
        tbar: [{
            xtype: 'button',
            id: 'add',
            text: UPLOAD_MANY_ONETIME,
            iconCls: 'icon-add',
            handler: function () {

                if (!win) {
                    win = Ext.create('Ext.window.Window', {
                        title: UPLOAD_MANY_ONETIME,
                        height: 450,
                        frame: false,
                        border: false,
                        width: 410,
                        listeners: {
                            close: function (e) {
                                win = undefined;
                            }
                        },
                        tbar: [{
                            html: '<input type="file" id="uploadify" name = "uploadify" />',
                            width: 118,
                            height: 35
                        }, {
                            xtype: 'button',
                            text: BEGIN_LOAD,
                            handler: function () {
                                $("#uploadify").uploadifyUpload();
                            }
                        }, {
                            xtype: 'button',
                            text: STOP_LOAD,
                            handler: function () {
                                $('#uploadify').uploadifyClearQueue();
                            }
                        }],
                        items: [{
                            html: "<div id='fileQueue' style='width: 450px;height: 400px;overflow: auto;border: 1px solid #E5E5E5;margin-bottom: 10px;'></div>"
                        }]
                    });
                }
                if (win.isVisible()) {
                    win.close(this);
                    win = undefined;
                }
                else {
                    win.show(this);
                }
                $("#uploadify").uploadify({
                    'uploader': '/Scripts/jquery.uploadify-v2.1.0/uploadify.swf',
                    'script': '/VendorProduct/upLoadImg?product_id=' + window.parent.GetProductId() + ';jsessionid=${pageContext.session.id}',
                    'cancelImg': '/Scripts/jquery.uploadify-v2.1.0/cancel.png',
                    'folder': 'UploadFile',
                    'queueID': 'fileQueue',
                    'fileExt': '*.gif;*.jpg;*.png',
                    'fileDesc': '*.gif;*.jpg;*.png',
                    //'buttonImg': '/img.gigade100.com/product/nopic_150.jpg', 
                    'buttonText': SELECT_IMG,
                    'auto': false,
                    'multi': true,
                    'scriptData': { ASPSESSID: window.parent.GethfAspSessID(), AUTHID: window.parent.GethfAuth() },
                    'onComplete': function (event, queueId, fileObj, response, data) {
                        var resText = eval("(" + response + ")");
                        if (resText[1] != undefined) {
                            Spec_Id = resText[1].spec_id;
                            addNewRow(resText[0].fileName);

                        } else {
                            //錯誤處理 
                            var index = resText.fileName.indexOf('/') + 1;
                            if (resText.fileName.split('/')[0] == "ERROR") {
                                if (data.fileCount > 0) {
                                    errorMsg += resText.fileName.substring(index, resText.fileName.length) + '<br/>';
                                }
                                else {
                                    errorMsg += resText.fileName.substring(index, resText.fileName.length);
                                }
                            }
                            else {
                                addNewRow(resText.fileName);
                            }
                        }
                        //延時執行close 
                        if (data.fileCount == 0) {
                            setTimeout(function () { win.close(); }, 500);
                            Ext.Msg.alert(PROMPT, errorMsg);
                            errorMsg = '';

                        }

                    }

                });

            }
        }]
    })



    Ext.create('Ext.Viewport', {
        layout: 'anchor',
        items: [picInfo],
        border: false,
        renderTo: Ext.getBody(),
        autoScroll: true,
        listeners: {
            resize: function () {
                this.doLayout()
            }
        }
    });


    StandardStore.load();
    explainStore.load();

    //權限 
    window.parent.updateAuth(picInfo, 'colName');


    Ext.Ajax.request({
        type: 'ajax',
        url: '/VendorProduct/QueryProduct',
        actionMethods: 'post',
        params: {
            "ProductId": Ext.htmlEncode(window.parent.GetProductId()),
            OldProductId: window.parent.GetCopyProductId()
        },
        success: function (response, opts) {
            var resText = eval("(" + response.responseText + ")");
            if (resText.data) {
                Pic.setSrc(resText.data.Product_Image);
                Ext.getCmp("product_media").setValue(resText.data.product_media);
                //edit by hufeng0813w 2014/05/28 圖檔的控件綁定上圖片的名稱 
                var filename = resText.data.Product_Image.substring(resText.data.Product_Image.lastIndexOf("/") + 1, resText.data.Product_Image.length);
                if (filename != "nopic_150.jpg")
                    Ext.getCmp("fileName").setValue(filename);
                else {
                }
            }
        }
    });
})

function imgFadeBig(img, width, height) {
    var e = this.event;
    if (img.split('/').length != 5) {
        $("#imgTip").attr("src", img)
.css({
    "top": (e.clientY < height ? e.clientY : e.clientY - height) + "px",
    "left": (e.clientX) + "px",
    "width": width + "px",
    "height": height + "px"
}).show();
    }

}





function addNewRow(imgPath) {
    var imgType = "";
    var imgInfo = imgPath.split('/');
    for (var i = 0; i < imgInfo.length - 1; i++) {
        //判斷圖片類型 
        if (imgInfo[i] == "product_spec") {
            //修改規格圖 
            var product_spec_info_alter = Ext.getCmp("standardPanel").store.data.items;
            for (var i = 0; i < product_spec_info_alter.length; i++) {
                if (product_spec_info_alter[i].get("spec_id") == Spec_Id) {
                    product_spec_info_alter[i].set('img', imgPath);
                }
            }
            break;
        }
        else if (imgInfo[i] == "product_picture") {
            var specGrid = Ext.getCmp("explainPanel").store.data.items;
            if (specGrid.length >= PIC_MAX) {
                Ext.Msg.alert(PROMPT, MAX_MSG);
                return;
            }
            var r = Ext.create('explain', {
                img: imgPath,
                image_sort: '0',
                image_state: '1'
            });
            explainStore.insert(0, r);
            cellEditingEx.startEditByPosition({ row: 0, column: 1 });
            break;
        }
        else if (imgInfo[i] == "product") {
            Pic.setSrc(imgPath);
            Ext.getCmp("fileName").setValue(imgInfo[imgInfo.length - 1]);
            break;
        } else if (imgInfo[0] == "ERROR") {
            Ext.Msg.alert(PROMPT, imgPath.split('/')[1]);
            break;
        }
    }
}

function addPic(nameType, value) {
    if (nameType == "desc") {
        var specGrid = Ext.getCmp("explainPanel").store.data.items;
        if (specGrid.length >= PIC_MAX) {
            Ext.Msg.alert(PROMPT, MAX_MSG);
            return;
        }
    }
    if (value != 0) {
        Spec_Id = value;
    }
    $(window.frames[0].document).find("#nameType").val(nameType);
    $(window.frames[0].document).find("#Filedata").click();
}

function saveTemp() {
    var mask;
    if (!mask) {
        mask = new Ext.LoadMask(Ext.getBody(), { msg: WAIT });
    }
    mask.show();
    //將數據寫入臨時表 
    if (!save()) {
        mask.hide();
        return;
    }
    Ext.Ajax.request({
        url: '/VendorProduct/SaveTemp',
        method: 'post',
        params: {
            ProductId: PRODUCT_ID,
            OldProductId: window.parent.GetCopyProductId()
        },
        success: function (response) {
            var data = eval("(" + response.responseText + ")");
            mask.hide();
            if (data.success) {
                Ext.Msg.alert(INFORMATION, data.msg, function () {
                    if (window.parent.GetCopyProductId() != '') {
                        //window.parent.history.go(-1); 
                        window.parent.parent.Ext.getCmp('ContentPanel').activeTab.close();
                    } else {
                        window.parent.parent.Ext.getCmp('ContentPanel').activeTab.update(window.top.rtnFrame('/VendorProduct/ProductSave'));
                    }
                });
            }
            else {
                Ext.Msg.alert(INFORMATION, data.msg);
            }
        }
    });
}

function save() {
    var retVal = true;
    //保存數據至數據庫 
    var product_image = Ext.getCmp("fileName").getValue();
    var product_spec_info = Ext.getCmp("standardPanel").store.data.items;
    var product_picture_info = Ext.getCmp("explainPanel").store.data.items;

    var image_InsertValue = product_image;
    var spec_InsertValue = "";
    var picture_InsertValue = "";
    var productMedia = Ext.getCmp("product_media").getValue();

    for (var i = 0; i < product_spec_info.length; i++) {
        var product_img = product_spec_info[i].get("img");
        var spec_name = product_spec_info[i].get("spec_id");
        var spec_sort = product_spec_info[i].get("spec_sort");
        var spec_status = product_spec_info[i].get("spec_status");
        //spec_status == "true" ? "1" : "0"; 
        if (spec_status == "true") {
            spec_status = 1;
        }
        if (spec_status == "false") {
            spec_status = 0;
        }
        product_img = product_img.substring(product_img.lastIndexOf("/") + 1); //圖片名稱!!!!! 
        if (product_img == undefined || product_img == "nopic_50.jpg") {
            product_img = "";
        }
        spec_InsertValue += product_img + "," + spec_name + "," + spec_sort + "," + spec_status + ";";
    }

    for (var i = 0; i < product_picture_info.length; i++) {
        var image_filename = product_picture_info[i].get("img");
        var image_sort = product_picture_info[i].get("image_sort");
        var image_state = product_picture_info[i].get("image_state");
        //image_state == "true" ? "1" : "0"; 
        if (image_state == "true") {
            image_state = 1;
        }
        if (image_state == "false") {
            image_state = 0;
        }
        picture_InsertValue += image_filename.substring(image_filename.lastIndexOf("/") + 1) + "," + image_sort + "," + image_state + ";";
    }

    Ext.Ajax.request({
        url: '/VendorProduct/productPictrueTempSave',
        method: 'POST',
        async: false,
        params: {
            "product_id": Ext.htmlEncode(window.parent.GetProductId()),
            OldProductId: window.parent.GetCopyProductId(),
            "image_InsertValue": Ext.htmlEncode(image_InsertValue),
            "spec_InsertValue": Ext.htmlEncode(spec_InsertValue),
            "picture_InsertValue": picture_InsertValue,
            "productMedia": productMedia
        },
        success: function (msg) {
            var resMsg = eval("(" + msg.responseText + ")");
            if (resMsg.success == true && resMsg.msg != null) {
                Ext.Msg.alert(PROMPT, resMsg.msg);

            }
            if (resMsg.success == false) {
                Ext.Msg.alert(PROMPT, resMsg.msg);
                retVal = false;
                window.parent.setMoveEnable(true);
            }
        }
    });
    window.parent.setMoveEnable(true);
    return retVal;

} 
