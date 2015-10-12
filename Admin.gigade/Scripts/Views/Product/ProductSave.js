/*  
 * 
 * 文件名称：ProductSave.js 
 * 摘    要：單一商品修改和新增 主要 保存頁面
 * 
 */
var CLICK_BTN_CHANGE = false;
var clickMovePrev = false;//上一步不進行保存驗證 add by xiangwang0413w 2015/01/06
var COURSEID = 0;

Ext.onReady(function () {
    var tabs = new Array();
    /******商品基本資料*************************************************************/
    var baseInfo = Ext.create('Ext.panel.Panel', {
        title: TITLE_BASE_INFO,
        //'<div style="width:100px">' + TITLE_BASE_INFO + '</div>',
        html: window.top.rtnFrame('/Product/baseInfo'),
        listeners: {
            beforeactivate: function () {
                setTimeout(function () { setMoveEnable(true) }, 2000);
            }
        }
    });
    tabs.push(baseInfo);

    /******物流配送模式**********************************************************************/
    var transportSet = Ext.create('Ext.panel.Panel', {
        title: PHYSICAL_DISTRIBUTION_DISPATCH_MODE,
        minWidth: 150,
        //'<div style="width:100px">' + '物流配送模式' + '</div>',
        listeners: {
            beforeactivate: function () {
                transportSet.update(window.top.rtnFrame('/Product/TransportSet'));
                setTimeout(function () { setMoveEnable(true) }, 2000);
            }
        }
    });
    tabs.push(transportSet);

    /******描述**********************************************************************/
    var description = Ext.create('Ext.panel.Panel', {
        title: TITLE_DESCRIPTION,
        html: window.top.rtnFrame('/Product/Description'),
        listeners: {
            beforeactivate: function () {
                setTimeout(function () { setMoveEnable(true) }, 2000);
            }
        }
    });
    tabs.push(description);

    /*******規格*********************************************************************/
    var spec = Ext.create('Ext.panel.Panel', {
        id: 'parentSpecPanel',
        title: TITLE_SPEC,
        //html: window.top.rtnFrame('/Product/SpecIndex'),
        listeners: {
            beforeactivate: function () {
                spec.update(window.top.rtnFrame('/Product/SpecIndex'));
                setTimeout(function () { setMoveEnable(true) }, 2000);
            }
        }
    });
    tabs.push(spec);

    /********課程**********************************************************************/
    var coursePanel = Ext.create('Ext.panel.Panel', {
        id: 'coursePanel',
        title: CURRICULUM,
        listeners: {
            beforeactivate: function () {
                coursePanel.update(window.top.rtnFrame('/Product/CourseDetailItem'));
                setTimeout(function () { setMoveEnable(true) }, 2000);
            }
        }
    });
    //tabs.push(courseDetailItem);

    /********價格**********************************************************************/
    var price = Ext.create('Ext.panel.Panel', {
        title: TITLE_PRICE,
        //html: window.top.rtnFrame('/Product/Price'),
        listeners: {
            beforeactivate: function () {
                price.update(window.top.rtnFrame('/Product/Price'));
                setTimeout(function () { setMoveEnable(true) }, 2000);
            }

        }
    });
    tabs.push(price);

    /*********類別**********************************************************************/
    var category = Ext.create('Ext.panel.Panel', {
        title: TITLE_CATEGORY,
        html: window.top.rtnFrame('/Product/CategoryIndex'),
        listeners: {
            beforeactivate: function () {
                setTimeout(function () { setMoveEnable(true) }, 2000);
            }
        }
    });
    tabs.push(category);

    /*******新類別**********************************************************************/
    var newcategory = Ext.create('Ext.panel.Panel', {
        title: NEW_PRODUCT_CATEGORY,
        html: window.top.rtnFrame('/Product/NewCategoryIndex'),
        listeners: {
            beforeactivate: function () {
                setTimeout(function () { setMoveEnable(true) }, 2000);
            }
        }
    });
    tabs.push(newcategory);

    /********庫存**********************************************************************/
    var stock = Ext.create('Ext.panel.Panel', {
        title: TITLE_STOCK,
        //html: window.top.rtnFrame('/Product/productStock'),
        listeners: {
            beforeactivate: function () {
                stock.update(window.top.rtnFrame('/Product/productStock'));
                setTimeout(function () { setMoveEnable(true) }, 2000);
                if (GetProductId() == '') {
                    Ext.getCmp('btnTempSave').show();
                }
            }
        }
    });
    tabs.push(stock);

    /*******圖檔*************************************************************************/
    var picture = Ext.create('Ext.panel.Panel', {
        title: TITLE_PICTURE,
        //html: window.top.rtnFrame('/Product/productPic'),
        listeners: {
            beforeactivate: function () {
                picture.update(window.top.rtnFrame('/Product/productPic'));
                setTimeout(function () { setMoveEnable(true) }, 2000);
                if (GetProductId() == '') {
                    Ext.getCmp('btnTempSave').show();
                }
            }
        }
    });
    tabs.push(picture);

    /*******抽獎**************************************************************************/
    var prize = Ext.create('Ext.panel.Panel', {
        title: TITLE_PRIZE,
        html: window.top.rtnFrame('/Product/FortuneIndex'),
        listeners: {
            beforeactivate: function () {
                setTimeout(function () { setMoveEnable(true) }, 2000);
                if (GetProductId() == '') {
                    Ext.getCmp('btnTempSave').show();
                }
            }
        }
    });
    tabs.push(prize);

    //單一商品保存
    var saveForm = Ext.create('Ext.tab.Panel', {
        id: 'ContentPanel',
        //width: 1185,
        minTabWidth: 110,
        tabPosition: 'bottom',
        activeTab: 0,
        title: '&nbsp;',
        layout: 'fit',
        items: [tabs],
        frame: true,
        buttons: [{
                text: BTN_SAVE,
                id: 'btnSave',
                hidden: GetProductId() == '',
                iconCls: 'icon-add',
                handler: function () {
                    saveForm.getActiveTab().body.dom.firstChild.contentWindow.save('btnSave');
                }
        }, {
            xtype: 'component',
            height: 25
        }, {
                text: BTN_TEMP_SAVE,
                id: 'btnTempSave',
                iconCls: 'icon-add',
                hidden: true,
                handler: function () {
                    saveForm.getActiveTab().body.dom.firstChild.contentWindow.saveTemp();
                }
        }],
        tbar: [{
                id: 'move-prev',
                iconCls: 'icon-prev',
                text: PERV_MOVE,
                handler: function () {
                    clickMovePrev = true;
                    CLICK_BTN_CHANGE = true;
                    var panel = Ext.getCmp('ContentPanel');
                    var t = panel.getActiveTab().prev();
                    if (t) {
                        panel.setActiveTab(t);
                        panel.doLayout();
                    }
                    CLICK_BTN_CHANGE = false;
                }
        }, '->', {
                id: 'move-next',
                iconCls: 'icon-next',
                iconAlign: 'right',
                text: NEXT_MOVE,
                handler: function () {
                    clickMovePrev = false;
                    CLICK_BTN_CHANGE = true;
                    var panel = Ext.getCmp('ContentPanel');
                    var t = panel.getActiveTab().next();
                    //if (t.title == "類別") {
                    //    var priceWindow = window.frames["price"];
                    //    priceWindow.Ext.getCmp('event_product_start')
                    //}
                    if (t) {
                        //if (t.title == "類別" && GetCopyProductId() != "") {
                        //    var priceWindow = window.frames[4];
                        //    priceWindow.Ext.getCmp('event_product_start');
                        //    Ext.Msg.confirm("消息提示", "您確認當前價格,時間嗎?", function (btn) {
                        //        if (btn == 'yes') {
                        //            panel.setActiveTab(t);
                        //            panel.doLayout();
                        //            CLICK_BTN_CHANGE = false;
                        //        } else {
                        //            return;
                        //        }
                        //    });
                        //}
                        //else {
                            panel.setActiveTab(t);
                            panel.doLayout();
                        //}
                    }
                    CLICK_BTN_CHANGE = false;
                }
        }],
        listeners: {
            beforetabchange: function (tabPanel, newCard, oldCard, eOpts) {
                if (Ext.getCmp('move-prev').isDisabled()) return false;
                if (GetProductId() == '') {
                    if (!CLICK_BTN_CHANGE) { return false; }
                    setMoveEnable(false);
                    if (newCard.title != TITLE_STOCK && newCard.title != TITLE_PICTURE && newCard.title != TITLE_PRIZE) {
                        Ext.getCmp('btnTempSave').hide();
                    }
                    if (!clickMovePrev && !oldCard.body.dom.firstChild.contentWindow.save()) {
                        setMoveEnable(true);
                        return false;
                    }
                }
                else {
                    if (newCard.title == TITLE_PRICE) {
                        Ext.getCmp('btnSave').hide();
                    }
                    else {
                        Ext.getCmp('btnSave').show();
                    }
                }
            }
        }
    });

    Ext.create('Ext.Viewport', {
        layout: 'anchor',
        items: [saveForm],
        renderTo: Ext.getBody(),
        autoScroll: true,
        listeners: {
            resize: function () {
                saveForm.height = document.documentElement.clientHeight;
                this.doLayout();
            },
            beforerender: function () {
                if (GetProductId() != '') {
                    QueryToolAuthorityByUrl('/Product/ProductSave');
                }
            }
        }
    });
});

function GetProductId() {
    return document.getElementById('ProductId').value;
}

function SetProductId(productId) {
    document.getElementById('ProductId').value = productId;
}

function GetCourseId() {
    return COURSEID;
}

function SetCourseId(courseId) {
    COURSEID = courseId;
}

function GetSavePanel() {
    return Ext.getCmp('ContentPanel');
}

function GetCoursePanel() {
    return Ext.getCmp('coursePanel');
}

/****************add by zhongyu0304w at 20131207****************************/
function GethfAuth() {
    return document.getElementById('hfAuth').value;
}

function GethfAspSessID() {
    return document.getElementById('hfAspSessID').value;
}
/*******************************end******************************************/

function GetCopyProductId() {
    return document.getElementById('OldProductId').value;
}

function GetBatchNo() {
    return document.getElementById('batchno').value;
}

//新增時無權限控制
function updateAuth(panel, str) {
    if (GetProductId() == '' || GetCopyProductId() != '') {
        var cols = panel.query('*[' + str + ']');
        Ext.Array.each(cols, function () { this.show(); });
    }
    else {
        setShow(panel, str);
    }
}

function setMoveEnable(status) {
    Ext.getCmp('move-prev').setDisabled(!status);
    Ext.getCmp('move-next').setDisabled(!status);
    //添加 按鈕  disabled 屬性 edit by zhuoqin0830w  2015/09/24
    Ext.getCmp('btnSave').setDisabled(!status);
}

function GetProduct(frm) {
    Ext.Ajax.request({
        url: '/Product/QueryProduct',
        method: 'POST',
        params: {
            ProductId: GetProductId(),
            OldProductId: GetCopyProductId()
        },
        success: function (form, action) {
            var result = Ext.decode(form.responseText);
            if (result.data) {
                frm.window.setForm(result.data);
            }
        },
        failure: function (form, action) {
            Ext.Msg.alert(INFORMATION, DATA_LOAD_FAILURE);
            return false;
        }
    });
}

function Is_Continue() {
    Ext.Msg.confirm(PROMPT, IS_CONTNUES, function (btn) {
        if (btn == 'no') {
            $(".x-tool-close").show();
            //刪除臨時表數據
            Ext.Ajax.request({
                url: '/Product/DeleteTempPro',
                method: 'post',
                params: {
                    OldProductId: GetCopyProductId()
                }
            });
        }
        else {
            window.frames[0].window.Page_Load();
        }
    });
    $(".x-tool-close").hide();
}