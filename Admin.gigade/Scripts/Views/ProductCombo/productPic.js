/*  
 * 
 * 文件名称：productPic.js 
 * 摘    要：組合商品修改和新增 圖檔頁面
 * 
 */
var PIC_MAX = 20;
var Spec_Id;
var imagesort;  // 定義一個公共的 imagesort 用來接收傳來的 數字  add by zhuoqin0830w 2015/01/29
var errorMsg = PIC_LIMIT;
var win;
var OLD_PRODUCT_ID;

//商品說明圖model
Ext.define("explain", {
    extend: 'Ext.data.Model',
    fields: [
        { name: "img", type: "string" },
        { name: "image_sort", type: "int" },
        { name: 'image_state', type: "string" },
        { name: 'pic_type', type: "int" },//圖片的類型 1:商品說明圖檔 2:手機說明圖檔
        {
            name: 'picType', type: 'string', convert: function (v, record) {//圖片的類型的中文 1:商品說明圖檔 2:手機說明圖檔
                if (record.data.pic_type == 1) {
                    return PRODUCT_EXPLAIN_PICS;//商品說明圖檔
                }
                else {
                    return PHONE_EXPLAIN_PICS;//手機說明圖檔
                }
            }
        }
    ]
});

//商品說明圖Store
var explainStore = Ext.create('Ext.data.Store', {
    model: 'explain',
    groupField: 'picType', //設置分組依據的列名 add by wwei0216w 2015/3/18 
    proxy: {
        type: 'ajax',
        url: '/ProductCombo/QueryExplainPic',
        actionMethods: 'post',
        reader: {
            type: 'json',
            root: 'items'
        }
    },
    sorters: [{//edit by xiangwang0413w 2014/10/17 根據序號降序排列
        property: 'image_sort',
        direction: 'DESC'
    }]
});

explainStore.on('beforeload', function () {
    Ext.apply(explainStore.proxy.extraParams,
        {
            product_id: Ext.htmlEncode(window.parent.GetProductId()),
            OldProductId: window.parent.GetCopyProductId()
        });
});

//MobileStore.on('beforeload', function () {
//    Ext.apply(MobileStore.proxy.extraParams,
//        {
//            product_id: Ext.htmlEncode(window.parent.GetProductId()),
//            OldProductId: window.parent.GetCopyProductId()
//        });
//});

var cellEditingEx = Ext.create('Ext.grid.plugin.CellEditing', {
    clicksToEdit: 1
});

var cellEditingMobile = Ext.create('Ext.grid.plugin.CellEditing', {
    clicksToEdit: 1
});

//複選框
var checkboxModel = Ext.create('Ext.selection.CheckboxModel', {
    listeners: {
        selectionchange: function (sm, selections) {
            Ext.getCmp("explainPanel").down('#delete').setDisabled(selections.length == 0);
        }
    }
});

//var mcheckboxModel = Ext.create('Ext.selection.CheckboxModel', {
//    listeners: {
//        selectionchange: function (sm, selections) {
//            Ext.getCmp("mobilePanel").down('#mdelete').setDisabled(selections.length == 0);
//        }
//    }
//});

var Pic;

///add by wwei0216w
///以下兩個方法用於為組合商品商品說明圖的右上角添加X按鈕 2015/7/13
function productPicEvent(picClick, img, type) {
    if (type == 1) {
        Ext.getCmp("fileName").setValue("");
    } else if (type == 2) {
        Ext.getCmp("fileNameMobile").setValue("");
    }
    $('.' + picClick).attr("src", img);
}
function SetClearPic(className, ImgId, cImgId, defaultImg) {
    $('.' + className).parent().parent().parent().parent().mousemove(function (e) {
        var target = e.target;
        //如果觸發對象為為IMG元素並且,它的id == "Pic_1"或者是clearPic_1 並且該圖片不是默認圖片,就顯示X小圖標
        if (target.tagName == 'IMG' && ($(target).attr("Id") == ImgId || $(target).attr("Id") == cImgId) && $(target).attr("src") != defaultImg) {
            $("#" + cImgId).show();
        } else {
            $("#" + cImgId).hide();
        }
    });
}

Ext.onReady(function () {
    var grouping = Ext.create('Ext.grid.feature.Grouping', {
        groupHeaderTpl: '{name}'
    }); //add by wwei0216w 2015/3/17 分組顯示圖片

    OLD_PRODUCT_ID = window.parent.GetCopyProductId();
    //商品圖
    Pic = Ext.create('Ext.Img', {
        width: 150,
        id: 'productPic',
        height: 150,
        floating: false,
        style: {
            margin: '10px 15px 0px 0px',//修改商品說明圖和手機說明圖之間的間距   edit by zhuoqin0830w 2015/03/19
            border: 'solid 1px #EEE',
            padding: '3 3 3 3'
        },
        cls: 'picClick', //為該圖片添加一個picClick的類 add by wwei0216w 20105/3/18
        listeners: {
            afterrender: function () {  //在該控件加載完成后
                $('.picClick').click(function () {  //通過picClick找到該控件的點擊事件
                    addPic("prod", 3); //進行圖片上傳的操作 3:商品說明圖
                });

                ///為商品圖添加一個img的x,其功能是刪去圖片
                ///productPicEvent的二個參數
                ///參數一:要刪除img控件的樣式(刪去圖片根據該控件的樣式來查找控件)
                ///參數二:用來替換用戶圖片的統一圖片路徑(通常為默認圖片)
                $('.picClick').parent().append("<img id='clearPic_1' onclick='productPicEvent(\"picClick\",\"/Content/img/點擊上傳.jpg\",1)' src='/Content/img/x.png' style='width:15px;height:15px;display:none;position:relative;z-index:0;cursor:pointer;top:12px;left:133px' />");
                $('.picClick').attr("Id", "Pic_1");
                SetClearPic("picClick", "Pic_1", "clearPic_1", "/Content/img/點擊上傳.jpg");
            }
        }
    });
    //手機商品圖
    Pic2 = Ext.create('Ext.Img', {
        width: 150,
        height: 150,
        //src: "/Content/img/點擊上傳圖片.jpg",
        style: {
            margin: '10px 0px 0px 0px',
            border: 'solid 1px #EEE',
            padding: '3 3 3 3'
        },
        cls: 'picClick2',//為該圖片添加一個picClic2k的類 add by wwei0216w 20105/3/18
        listeners: {//在該控件加載完成后
            afterrender: function () {
                $('.picClick2').click(function () {//通過picClick2找到該控件的點擊事件
                    addPic("mobile", 4);//進行圖片上傳的操作 4：手機說明圖
                });
                $('.picClick2').parent().append("<img id='clearPic_2' onclick='productPicEvent(\"picClick2\",\"/Content/img/點擊上傳圖片.jpg\",2)' src='/Content/img/x.png' style='width:15px;height:15px;display:none;position:relative;z-index:0;cursor:pointer;top:12px;left:298px' />");
                $('.picClick2').attr("Id", "Pic_2");
                SetClearPic("picClick2", "Pic_2", "clearPic_2", "/Content/img/點擊上傳圖片.jpg");
            }
        }
    });

    //商品說明圖  
    var expalinPic = Ext.create("Ext.grid.Panel", {
        plugins: [cellEditingEx],
        selModel: checkboxModel,
        y: 5,
        id: 'explainPanel',
        store: explainStore,
        columnLines: true,
        features: [grouping],
        width: 700,
        height: 300,
        border: true,
        defaults: {
            labelWidth: 80
            //,
            //padding: '5 0 0 5'
        },
        tbar: [
             // add by zhuoqin0830w  2015/03/27  添加 商品說明圖 和 手機說明圖 全選按鈕
            { xtype: 'button', text: PRODUCT_EXPLAIN_PIC_SELECT_ALL, id: 'selectExplain', iconCls: '', handler: function () { SelectAll("explain") } },//商品說明圖全選
            { xtype: 'button', text: PHONE_EXPLAIN_PIC_SELECT_ALL, id: 'selectMobile', iconCls: '', handler: function () { SelectAll("mobile") } },//手機說明圖全選
            '->',
            { xtype: 'button', text: DELETE, id: 'delete', iconCls: 'ui-icon ui-icon-error', disabled: true, handler: deletePic }
        ],
        columns: [{
            xtype: 'actioncolumn',
            width: 100,
            id: 'deleteEx',
            //colName: 'deleteEx',
            hidden: true,
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
                        url: '/ProductCombo/DeletePic',
                        method: 'post',
                        params: {
                            "type": 'explain',
                            "rec": rec,
                            "product_id": '',
                            "apporexplain": 1
                        }
                    });
                }
            }]
        }, {
            header: PRODUCT_EXPLAIN_PIC,
            sortable: false,
            menuDisabled: true,
            dataIndex: 'image_filename',
            hidden: true,
            colName: 'image_filename',
            id: 'image_filename',
            xtype: 'templatecolumn',
            tpl: '<img width=50 height=50 onmousemove="javascript:imgFadeBig(this.src,250);" onmouseout = "javascript:$(\'#imgTip\').hide()" src="{img}" />',
            width: 200,
            align: "center"
        }, {
            header: PIC_SORT,
            sortable: false,
            menuDisabled: true,
            width: 165,
            colName: 'image_sort',
            dataIndex: 'image_sort',
            hidden: true,
            id: 'image_sort',
            align: 'center',
            editor: {
                xtype: 'textfield'
            }
        }, {
            header: PIC_SHOW,
            sortable: false,
            menuDisabled: true,
            width: 165,
            colName: 'image_state',
            dataIndex: 'image_state',
            hidden: true,
            id: 'image_state',
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
        width: 1000,
        height: 650,
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
            hidden: true,
            items: [Pic, Pic2, {
                xtype: 'panel',
                width: 900,
                height: 150,
                border: false,
                layout: 'absolute',
                items: [
                    //{
                    //xtype: 'label',
                    //text: PRODUCT_PIC,
                    //x: 10,
                    //y: 70
                    //},
                {
                    xtype: 'textfield',
                    id: 'fileName',
                    width: 280,
                    hidden: true,
                    readOnly: true,
                    x: 60,
                    y: 70
                }, {
                    xtype: 'textfield',
                    id: 'fileNameMobile',
                    hidden: true,
                    width: 280,
                    readOnly: true,
                    x: 60,
                    y: 70
                },
                //{
                //    xtype: 'button',
                //    text: SELECT_IMG,
                //    x: 360,
                //    y: 70,
                //    handler: function () {
                //        addPic("prod", 1);
                //    }
                //},
                {
                    xtype: 'label',
                    html: "<span style='font-size:10; color:Gray'>" + MESSAGE + "</span>",
                    x: 10,
                    y: 80
                }]
            }]
        }, {
            xtype: 'panel',
            border: false,
            layout: 'hbox',
            items: [{
                xtype: 'displayfield',
                margin: '0px 0px 0px 25px',
                value: PRODUCT_PIC//商品圖
            }, {
                xtype: 'displayfield',
                margin: '0px 0px 0px 60px',
                value: PHONE_EXPLAIN_IMG//手機商品圖
            }]
        }, {
            xtype: 'panel',
            border: false,
            layout: 'column',
            items: [{
                xtype: 'textfield',
                id: 'product_alt',
                fieldLabel: IMAGE_EXPLAIN,//圖片說明
                labelWidth: 70,
                border: false,
                width: 480
            }, {
                xtype: 'panel',
                width: 900,
                border: false,
                colName: 'product_media',
                hidden: true,
                layout: 'hbox',
                items: [{
                    xtype: 'textfield',
                    id: 'product_media',
                    fieldLabel: PRODUCT_MEDIA,//商品影片
                    labelWidth: 70,
                    border: false,
                    width: 480
                }, {
                    xtype: 'displayfield',
                    id: 'dis',
                    value: '<span style="color:gray">*</span><a id="yTE" href="#" >' + YouTube_INSET_CODE_EXPLAIN + '</a>',//YouTube嵌入程序碼取得說明
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
            }]
        }, {
            xtype: 'button',
            text: ADD_EXPLAIN_PIC,
            id: 'ExpUpload',
            hidden: true,
            colName: 'ExpUpload',
            y: 5,
            x: 5,
            iconCls: 'icon-add',
            handler: function () { addPic("desc", 1); }
        }, {
            xtype: 'button',
            text: ADD_MOBILE_PIC,
            id: 'MobileUpload',
            defaults: {
                labelWidth: 80,
                padding: '5 0 0 5'
            },
            //colName: 'ExpUpload',
            //hidden: true,
            x: 30,
            y: 5,
            iconCls: 'icon-add',
            handler: function () { addPic("app", 2); }
        }, expalinPic],
        tbar: [{
            xtype: 'button',
            id: 'add',
            text: UPLOAD_MANY_ONETIME,
            iconCls: 'icon-add',
            handler: function () {
                if (!win) {
                    win = Ext.create('Ext.window.Window', {
                        title: UPLOAD_MANY_ONETIME,
                        height: 420,
                        frame: false,
                        border: false,
                        width: 400,
                        listeners: {
                            close: function () {
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
                            html: "<div id='fileQueue' style='width: 395px;height: 415px;overflow: auto;border: 1px solid #E5E5E5;margin-bottom: 10px;'></div>"
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
                    'script': '/ProductCombo/upLoadImg?product_id=' + window.parent.GetProductId() + ';jsessionid=${pageContext.session.id}',
                    'cancelImg': '/Scripts/jquery.uploadify-v2.1.0/cancel.png',
                    'folder': 'UploadFile',
                    'queueID': 'fileQueue',
                    'fileExt': '*.gif;*.jpg;*.png',
                    //'buttonImg': '/img.gigade100.com/product/nopic_150.jpg',
                    'fileDesc': '*.gif;*.jpg;*.png',
                    'buttonText': SELECT_IMG,
                    'auto': false,
                    'multi': true,
                    'scriptData': { ASPSESSID: window.parent.GethfAspSessID(), AUTHID: window.parent.GethfAuth() },
                    'onComplete': function (event, queueId, fileObj, response, data) {
                        //$("#uploadifyUploader").hide();  //add by wwei2014/11/14 解決上傳一張圖片后上傳按鈕消失
                        var resText = eval("(" + response + ")");
                        var msg = "";
                        imagesort = resText.image_sort;  //接收數字并賦給全局變量 add by zhuoqin0830w 2015/01/29
                        if (resText[1] != undefined) {
                            Spec_Id = resText[1].spec_id;
                            msg = addNewRow(resText[0].fileName);
                        } else {
                            //錯誤處理
                            var index = resText.fileName.indexOf('/') + 1;
                            if (resText.fileName.split('/')[0] == "ERROR") {
                                if (data.fileCount > 0) {
                                    errorMsg += resText.fileName.substring(index, resText.fileName.length) + '<br/>';
                                }
                                else {
                                    errorMsg += resText.fileName.substring(index, resText.fileName.length) + '<br/>';

                                }
                            }
                            else {
                                msg = addNewRow(resText.fileName, resText.Type);
                            }
                        }
                        if (data.fileCount == 0) {
                            setTimeout(function () { win.close(); }, 500);

                            //add by zhuoqin0830w  2015/03/24  判斷上傳的圖片中是否有重複排序
                            CheckNum(msg);

                            //判斷錯誤信息是否等於 初始值 edit by zhuoqin0830w 2015/02/06
                            if (errorMsg != PIC_LIMIT) {
                                Ext.Msg.alert(PROMPT, errorMsg);
                                errorMsg = PIC_LIMIT;  //使 errorMsg 賦值為空 并將賦值為初始值  edit by zhuoqin0830w 2015/02/05
                            }
                        }
                    }
                });
            }
        }]
    });

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

    explainStore.load({ params: { 'apporexplain': 1 } });
    //MobileStore.load({ params: { 'apporexplain': 2 } });
    //權限
    window.parent.updateAuth(picInfo, 'colName');

    Ext.Ajax.request({
        type: 'ajax',
        url: '/ProductCombo/QueryProduct',
        actionMethods: 'post',
        params: {
            "ProductId": Ext.htmlEncode(window.parent.GetProductId()),
            "OldProductId": OLD_PRODUCT_ID
        },
        success: function (response, opts) {
            var resText = eval("(" + response.responseText + ")");
            //if (!resText) return;
            //Pic.setSrc(resText.data.Product_Image);
            //Ext.getCmp("product_media").setValue(resText.data.product_media);
            ////edit by hufeng0813w 2014/05/28 圖檔的控件綁定上圖片的名稱
            //var filename = resText.data.Product_Image.substring(resText.data.Product_Image.lastIndexOf("/") + 1, resText.data.Product_Image.length);
            //if (filename != "nopic_150.jpg")
            //    Ext.getCmp("fileName").setValue(filename);
            if (resText.data) {
                var filename = resText.data.Product_Image.substring(resText.data.Product_Image.lastIndexOf("/") + 1, resText.data.Product_Image.length);//獲得商品說明圖的名稱 add by wwei0216w 2015/3/18
                var mobileFilename = resText.data.Mobile_Image.substring(resText.data.Mobile_Image.lastIndexOf("/") + 1, resText.data.Mobile_Image.length);//獲得手機說明圖的名稱 add by wwei0216w 2015/3/18
                if (filename != "nopic_150.jpg") {          //如果說明圖名稱不等於"nopic_150.jpg",證明存在圖片
                    Pic.setSrc(resText.data.Product_Image); //設置圖片路徑
                    Ext.getCmp("fileName").setValue(filename); //將圖片路徑保存到fileName中
                } else {
                    //Pic.setSrc("/Content/img/點擊上傳.jpg");//如果說明圖名稱等於"nopic_150.jpg",將圖片換成預設圖片
                    $('.picClick').attr("src", "/Content/img/點擊上傳.jpg");
                }
                if (mobileFilename != "nopic_150.jpg") {   //如果手機說明圖名稱不等於"nopic_150.jpg",證明存在圖片
                    Pic2.setSrc(resText.data.Mobile_Image);//設置圖片路徑
                    Ext.getCmp("fileNameMobile").setValue(mobileFilename);//將圖片路徑保存到fileNameMobile中
                } else {
                    Pic2.setSrc("/Content/img/點擊上傳圖片.jpg");//如果說明圖名稱等於"nopic_150.jpg",將圖片換成預設圖片
                }

                //edit by zhuoqin0830w  2014/04/14
                Ext.getCmp("product_media").setValue(resText.data.product_media);
                Ext.getCmp("product_alt").setValue(resText.data.Product_alt);
            }
        }
    });
})

//function addNewRow(imgPath, type) {
//    var imgType = "";
//    var imgInfo = imgPath.split('/');
//    for (var i = 0; i < imgInfo.length - 1; i++) {
//        //判斷圖片類型
//        if (imgInfo[i] == "product_picture") {
//            var specGrid = Ext.getCmp("explainPanel").store.data.items;
//            if (specGrid.length >= PIC_MAX) {
//                Ext.Msg.alert(PROMPT, MAX_MSG);
//                return;
//            }
//            var r = Ext.create('explain', {
//                img: imgPath,
//                image_sort: imagesort,//edit by zhuoqin0830w 2015/01/29  將“0”轉換為圖片上的數字
//                image_state: '1'
//            });
//            (type == 1) ? explainStore.insert(0, r) : MobileStore.insert(0, r);
//            cellEditingEx.startEditByPosition({ row: 0, column: 1 });
//            break;
//        }
//        else if (imgInfo[i] == "product") {
//            Pic.setSrc(imgPath);
//            Ext.getCmp("fileName").setValue(imgInfo[imgInfo.length - 1]);
//            break;
//        } else if (imgInfo[0] == "ERROR") {
//            Ext.Msg.alert(PROMPT, imgInfo[imgInfo.length - 1]);
//            break;
//        }
//    }
//}

function addNewRow(imgPath, type) {
    var imgType = "";
    var imgInfo = imgPath.split('/');
    for (var i = 0; i < imgInfo.length - 1; i++) {
        //edit by zhuoqin0830w  2015/03/26  分別判斷 手機說明圖 和 商品說明圖 上傳的最大張數是否 大於 20 張
        //初始值設定 
        var explain = 1, mobile = 1, data = Ext.getCmp("explainPanel").getStore().data;
        //遍歷循環 grid 中所有的數據  從 0 開始
        for (var image = 0; image < data.length; image++) {
            //判斷 說明圖檔 從 0 開始的數據中 查看說明圖檔共有多少張 相片
            if (type == 1) {
                if (data.items[image].data.pic_type == 1) {
                    explain++;
                }
            }
            //判斷 手機圖檔 從 0 開始的數據中 查看手機圖檔共有多少張 相片
            if (type == 2) {
                if (data.items[image].data.pic_type == 2) {
                    mobile++;
                }
            }
        }

        if (explain > PIC_MAX) {
            Ext.Msg.alert(PROMPT, MAX_MSG);
            $('#uploadify').uploadifyClearQueue();
            return "error";
        }
        if (mobile > PIC_MAX) {
            Ext.Msg.alert(PROMPT, MAX_MOBILE);
            $('#uploadify').uploadifyClearQueue();
            return "error";
        }
        if (imgInfo[0] == "ERROR") {
            Ext.Msg.alert(PROMPT, imgPath.split('/')[1]);
            break;
        }

        var r = Ext.create('explain', {
            img: imgPath,
            image_sort: imagesort,//edit by zhuoqin0830w 2015/01/29  將“0”轉換為圖片上的數字
            image_state: '1',
            pic_type: 1
        });
        var d = Ext.create('explain', {
            img: imgPath,
            image_sort: imagesort,//edit by zhuoqin0830w 2015/01/29  將“0”轉換為圖片上的數字
            image_state: '1',
            pic_type: 2
        });
        if (type == 1) {
            explainStore.insert(0, r);
        } else if (type == 2) {
            explainStore.insert(0, d);
        } else if (type == 3) {
            Pic.setSrc(imgPath);
            Ext.getCmp("fileName").setValue(imgInfo[imgInfo.length - 1]);//edit by wwei0216w 2015/03/18 將商品說明圖的路徑放入fileName中,方便保存時讀取
            break;// add by zhuoqin0830w  當商品圖上傳成功之後返回判斷是否還有圖片上傳
        }
        else if (type == 4) {
            Pic2.setSrc(imgPath);
            Ext.getCmp("fileNameMobile").setValue(imgInfo[imgInfo.length - 1]);//edit by wwei0216w 2015/03/18 將手機說明圖的路徑放入fileNameMobile中,方便保存時讀取
            break;
        }
        cellEditingEx.startEditByPosition({ row: 0, column: 1 });
        break;
    }
}

//function addPic(nameType) {
//    if (nameType == "desc") {
//        var specGrid = Ext.getCmp("explainPanel").store.data.items;
//        if (specGrid.length >= PIC_MAX) {
//            Ext.Msg.alert(PROMPT, MAX_MSG);
//            return;
//        }
//    }
//    $(window.frames[0].document).find("#nameType").val(nameType);
//    $(window.frames[0].document).find("#Filedata").click();
//}

function addPic(nameType, appOrexplain) {
    //判斷商品說明圖的張數是否大於20  edit by zhuoqin0830w  2015/03/26
    var data = Ext.getCmp("explainPanel").getStore().data;
    if (nameType == "desc") {
        var explain = 0;
        for (var image = 0; image < data.length; image++) {
            if (data.items[image].data.pic_type == 1) {
                explain++;
            }
        }
        if (explain >= PIC_MAX) {
            Ext.Msg.alert(PROMPT, MAX_MSG);
            return;
        }
    }
    //判斷手機說明圖的張數是否大於20  add by zhuoqin0830w  2015/03/26
    if (nameType == "app") {
        var mobile = 0;
        for (var image = 0; image < data.length; image++) {
            if (data.items[image].data.pic_type == 2) {
                mobile++;
            }
        }
        if (mobile >= PIC_MAX) {
            Ext.Msg.alert(PROMPT, MAX_MOBILE);
            return;
        }
    }

    imagesort = 0; //新增單個的時候避免將值傳入賦為初始值0  add by zhuoqin0830w 2015/01/29
    $(window.frames[0].document).find("#nameType").val(nameType);
    $(window.frames[0].document).find("#appOrexplain").val(appOrexplain);
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
    //臨時表批量更新到正式表
    Ext.Ajax.request({
        url: '/ProductCombo/Temp2Pro',
        method: 'post',
        params: {
            OldProductId: window.parent.GetCopyProductId()
        },
        success: function (response) {
            var data = eval("(" + response.responseText + ")");
            mask.hide();
            if (data.success) {
                Ext.Msg.alert(INFORMATION, data.msg, function () {
                    if (OLD_PRODUCT_ID != '') {
                        //window.parent.history.go(-1);
                        window.parent.parent.Ext.getCmp('ContentPanel').activeTab.close();
                    } else {
                        window.parent.parent.Ext.getCmp('ContentPanel').activeTab.update(window.top.rtnFrame('/ProductCombo/Index'));
                    }
                });
            }
            else {
                Ext.Msg.alert(INFORMATION, data.msg);
            }
        }
    });
}

function imgFadeBig(img, size) {
    var e = this.event;
    var topValue;
    $("#imgTip").attr("src", img).css({
        "top": (e.clientY - size) < 0 ? e.clientY : (e.clientY - size) + "px",
        "left": e.clientX + "px",
        "width": size + "px",
        "height": size + "px"
    }).show();
}

function save() {
    //添加 遮罩層  避免用戶多次點擊  edit by zhuoqin0830w  2015/09/24
    var mask;
    if (!mask) {
        mask = new Ext.LoadMask(Ext.getBody(), { msg: WAIT });
    }
    mask.show();
    //添加disabled屬性  避免用戶多次點擊  edit by zhuoqin0830w  2015/09/24
    window.parent.setMoveEnable(false);

    var retVal = true;
    //保存數據至數據庫
    var product_image = Ext.getCmp("fileName").getValue();
    var product_picture_info = Ext.getCmp("explainPanel").store.data.items;
    var mobile_image = Ext.getCmp("fileNameMobile").getValue();//edit by wwei0216w 2015/03/18 將手機說明圖的路徑讀取
    var pic_specify = Ext.getCmp("product_alt").getValue();//edit by wwei0216w 2015/04/9 商品圖片說明
    //var product_mobilepic_info = Ext.getCmp("mobilePanel").store.data.items;  // add by wwei0216w 2014/11/11獲取mobilePanel中的所有數據

    var image_InsertValue = product_image;
    var image_MobileValue = mobile_image;
    var specify_Product_alt = pic_specify; // edit by wwei0216w 2015/04/9
    var picture_InsertValue = "";
    var mobilePic_InsertValue = "";  //用來記錄mobilPanel中的數據
    var productMedia = Ext.getCmp("product_media").getValue();

    ////將手機圖片的相關信息拼接成字符串 add by  wwei 0216w
    //for (var i = 0; i < product_mobilepic_info.length; i++) {
    //    var image_filename = product_mobilepic_info[i].get("img");
    //    var image_sort = product_mobilepic_info[i].get("image_sort");
    //    var image_state = product_mobilepic_info[i].get("image_state");
    //    if (image_state == "true") {
    //        image_state = 1;
    //    }
    //    if (image_state == "false") {
    //        image_state = 0;
    //    }
    //    mobilePic_InsertValue += image_filename.substring(image_filename.lastIndexOf("/") + 1) + "," + image_sort + "," + image_state + ";";
    //}

    for (var i = 0; i < product_picture_info.length; i++) {
        var image_filename = product_picture_info[i].get("img");
        var image_sort = product_picture_info[i].get("image_sort");
        var image_state = product_picture_info[i].get("image_state");
        var pic_type = product_picture_info[i].get("pic_type"); //獲取pic_type對同一個store里德圖片進行分組 add by wwei0216w 2015/3/18
        //image_state == "true" ? "1" : "0";
        if (image_state == "true") {
            image_state = 1;
        }
        if (image_state == "false") {
            image_state = 0;
        }
        if (pic_type == 1) {//商品圖
            picture_InsertValue += image_filename.substring(image_filename.lastIndexOf("/") + 1) + "," + image_sort + "," + image_state + ";";
        } else if (pic_type == 2) {//手機圖
            mobilePic_InsertValue += image_filename.substring(image_filename.lastIndexOf("/") + 1) + "," + image_sort + "," + image_state + ";";
        }
    }

    Ext.Ajax.request({
        url: '/ProductCombo/productPictrueTempSave',
        method: 'POST',
        async: window.parent.GetProductId() == '' ? false : true,
        params: {
            "product_id": Ext.htmlEncode(window.parent.GetProductId()),
            "OldProductId": OLD_PRODUCT_ID,
            "image_InsertValue": Ext.htmlEncode(image_InsertValue),
            "image_MobileValue": Ext.htmlEncode(image_MobileValue),//傳遞手機說明圖
            "specify_Product_alt": Ext.htmlEncode(specify_Product_alt),//add by wwei0216w 2015/04/09
            "picture_InsertValue": picture_InsertValue,
            "productMedia": productMedia,
            "mobilePic_InsertValue": mobilePic_InsertValue
        },
        success: function (msg) {
            var resMsg = eval("(" + msg.responseText + ")");
            mask.hide();
            if (resMsg.success == true && resMsg.msg != null) {
                Ext.Msg.alert(PROMPT, resMsg.msg);
            }
            if (resMsg.success == false) {
                Ext.Msg.alert(PROMPT, resMsg.msg);
                retVal = false;
            }
            window.parent.setMoveEnable(true);
        }
    });
    //window.parent.setMoveEnable(true);
    return retVal;
}

function deletePic() {
    var rows = Ext.getCmp("explainPanel").getSelectionModel().getSelection();
    Ext.Msg.confirm(CONFIRM, Ext.String.format(DELETE_INFO, rows.length), function (btn) {
        if (btn == 'yes') {
            explainStore.remove(rows);
        }
    });
}

//function mdeletePic() {
//    var rows = Ext.getCmp("mobilePanel").getSelectionModel().getSelection();
//    Ext.Msg.confirm(CONFIRM, Ext.String.format(DELETE_INFO, rows.length), function (btn) {
//        if (btn == 'yes') {
//            MobileStore.remove(rows);
//        }
//    });
//}

//add by zhuoqin0830w  2015/03/24  判斷上傳的圖片中是否有重複排序
function CheckNum(msg) {
    //添加判斷  上傳的商品 說明圖檔 和 手機圖檔 是否有重複排序  eidt by zhuoqin0830w  2015/03/24
    var explain = 0, mobile = 0, explainimages, data = Ext.getCmp("explainPanel").getStore().data;
    //遍歷循環 grid 中所有的數據  從 0 開始
    for (var image = 0; image < data.length; image++) {
        explainimages = image + 1;
        //遍歷循環 grid 中所有的  從 1 開始
        for (explainimages; explainimages < data.length; explainimages++) {
            //判斷 說明圖檔 從 0 開始的數據中 圖片類型 和 從 1 開始的數據中 圖片類型 是否相同  如果相同則 判斷 所得到的 序號是否相同
            if (data.items[image].data.pic_type == 1 && data.items[explainimages].data.pic_type == 1) {
                if (data.items[image].data.image_sort == data.items[explainimages].data.image_sort) {
                    explain++;
                }
            }
            //判斷 手機圖檔 從 0 開始的數據中 圖片類型 和 從 1 開始的數據中 圖片類型 是否相同  如果相同則 判斷 所得到的 序號是否相同
            if (data.items[image].data.pic_type == 2 && data.items[explainimages].data.pic_type == 2) {
                if (data.items[image].data.image_sort == data.items[explainimages].data.image_sort) {
                    mobile++;
                }
            }
        }
    }
    if (explain > 0 && msg != "error") {
        errorMsg += PRODUCT_HAVE_REPEAT_IMG_ORDER + '<br/>';//商品說明圖中有重複的商品排序
    }
    if (mobile > 0 && msg != "error") {
        errorMsg += PRODUCT_PHONE_HAVE_REPEAT_IMG_ORDER + '<br/>';//商品手機圖中有重複的商品排序
    }
}

// add by zhuoqin0830w  2015/03/27  添加 商品說明圖 和 手機說明圖 全選按鈕
function SelectAll(type) {
    var data = Ext.getCmp("explainPanel").getStore().data;
    var selMod = Ext.getCmp("explainPanel").getSelectionModel();
    if (type == "explain") {
        for (var image = 0; image < data.length; image++) {
            if (data.items[image].data.pic_type == 1) {
                selMod.select(image, true, false);
            } else { selMod.deselect(image, true, false); }
        }
    } else if (type == "mobile") {
        for (var image = 0; image < data.length; image++) {
            if (data.items[image].data.pic_type == 2) {
                selMod.select(image, true, false);
            } else { selMod.deselect(image, true, false); }
        }
    }
}