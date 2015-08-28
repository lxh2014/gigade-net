var CLICK_BTN_CHANGE = false; /*是否提交的全局變量*/

Ext.onReady(function () {

    var tabs = new Array();
    //商品基本資料
    var baseInfo = Ext.create('Ext.panel.Panel', {
        title: TITLE_BASE_INFO,
        html: rtnFrame('/VendorProductCombo/BaseInfo'),
        listeners: {
            beforeactivate: function () {
                setTimeout(function () { setMoveEnable(true) }, 2000);
            }
        }
    });
    tabs.push(baseInfo); /*添加入tab*/

    //描述
    var description = Ext.create('Ext.panel.Panel', {
        title: TITLE_DESCRIPTION,
        html: window.top.rtnFrame('/VendorProductCombo/Description'),
        listeners: {
            beforeactivate: function () {
                setTimeout(function () { setMoveEnable(true) }, 2000);
            }
        }
    });
    tabs.push(description);

    //規格
    var spec = Ext.create('Ext.panel.Panel', {
        title: TITLE_SPEC,
        html: window.top.rtnFrame('/VendorProductCombo/Spec'),
        listeners: {
            beforeactivate: function () {
                spec.update(window.top.rtnFrame('/VendorProductCombo/Spec'));
                setTimeout(function () { setMoveEnable(true) }, 2000);
            }
        }
    });
    tabs.push(spec);

    //價格
    var price = Ext.create('Ext.panel.Panel', {
        title: TITLE_PRICE,
        html: window.top.rtnFrame('/VendorProductCombo/Price'),
        listeners: {
            beforeactivate: function () {
                price.update(window.top.rtnFrame('/VendorProductCombo/Price'));
                setTimeout(function () { setMoveEnable(true) }, 2000);
            }
        }
    });
    tabs.push(price);

    //類別
    var category = Ext.create('Ext.panel.Panel', {
        title: TITLE_CATEGORY,
        html: window.top.rtnFrame('/VendorProductCombo/Category'),
        listeners: {
            beforeactivate: function () {
                setTimeout(function () { setMoveEnable(true) }, 2000);
            }
        }
    });
    tabs.push(category);

    //庫存
    var stock = Ext.create('Ext.panel.Panel', {
        title: TITLE_STOCK,
        html: window.top.rtnFrame('/VendorProductCombo/productStock'),
        listeners: {
            beforeactivate: function () {
                stock.update(window.top.rtnFrame('/VendorProductCombo/productStock'));
                setTimeout(function () { setMoveEnable(true) }, 2000);
                if (GetIsEdit() == "false") {//獲取不到pid時為新增，顯示儲存，將數據加入product_temp 
                    Ext.getCmp('btnTempSave').show();
                }
            }
        }
    });
    tabs.push(stock);

    //圖檔
    var picture = Ext.create('Ext.panel.Panel', {
        title: TITLE_PICTURE,
        html: window.top.rtnFrame('/VendorProductCombo/productPic'),
        listeners: {
            beforeactivate: function () {
                picture.update(window.top.rtnFrame('/VendorProductCombo/productPic'));
                setTimeout(function () { setMoveEnable(true) }, 2000);
                if (GetIsEdit() == "false") {//獲取不到pid時為新增，顯示儲存，將數據加入product_temp 
                    Ext.getCmp('btnTempSave').show();
                }
            }
        }
    });
    tabs.push(picture);

    /*panel*/
    var saveForm = Ext.create('Ext.tab.Panel', {
        id: 'ContentPanel',
        //width: 1185,
        minTabWidth: 100,
        tabPosition: 'bottom',
        activeTab: 0,
        title: '&nbsp;',
        layout: 'fit',
        items: [tabs],
        frame: true,
        buttons: [
            {
                text: BTN_SAVE, /*編輯 保存 */
                id: 'btnSave',
                hidden: GetIsEdit() == "false",
                iconCls: 'icon-add',
                handler: function () {
                    saveForm.getActiveTab().body.dom.firstChild.contentWindow.save('btnSave');
                }
            },
            { xtype: 'component', height: 25 },
            {
                text: BTN_TEMP_SAVE, /*新增 儲存 */
                id: 'btnTempSave',
                iconCls: 'icon-add',
                hidden: true,
                handler: function () {
                    saveForm.getActiveTab().body.dom.firstChild.contentWindow.saveTemp();
                }
            }
        ],
        tbar: [
            {
                id: 'move-prev',
                iconCls: 'icon-prev',
                text: PERV_MOVE, /*上一步 */
                handler: function () {
                    CLICK_BTN_CHANGE = true;
                    var panel = Ext.getCmp('ContentPanel');
                    var t = panel.getActiveTab().prev();
                    if (t) {
                        panel.setActiveTab(t);
                        panel.doLayout();
                    }
                    CLICK_BTN_CHANGE = false;
                }
            },
            '->',
            {
                id: 'move-next',
                iconCls: 'icon-next',
                iconAlign: 'right',
                text: NEXT_MOVE, /*下一步 */
                handler: function () {
                    CLICK_BTN_CHANGE = true;
                    var panel = Ext.getCmp('ContentPanel');
                    var t = panel.getActiveTab().next();
                    if (t) {
                        panel.setActiveTab(t);
                        panel.doLayout();
                    }
                    CLICK_BTN_CHANGE = false;
                }
            }
        ],
        listeners: {
            beforetabchange: function (tabPanel, newCard, oldCard, eOpts) {
                /*保存和儲存動作 儲存和保存按鈕的轉換*/

                if (Ext.getCmp('move-prev').isDisabled()) return false;
                if (GetIsEdit() == 'false') {
                    if (!CLICK_BTN_CHANGE) { return false; }
                    setMoveEnable(false);

                    if (newCard.title != TITLE_STOCK && newCard.title != TITLE_PICTURE && newCard.title != TITLE_PRIZE) {
                        Ext.getCmp('btnTempSave').hide();
                    }

                    if (!oldCard.body.dom.firstChild.contentWindow.save()) {
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
                    QueryToolAuthorityByUrl('/VendorProductCombo');
                }
            }
        }
    });
});

function GetProductId() {
    return document.getElementById('ProductId').value;
}
function SetProductId(value) {
    document.getElementById('ProductId').value = value;
}

function GetIsEdit() {
    return document.getElementById('IsEdit').value;
}

function GetCopyProductId() {
    return document.getElementById('OldProductId').value;
}

function GethfAuth() {
    return document.getElementById('hfAuth').value;
}

function GethfAspSessID() {
    return document.getElementById('hfAspSessID').value;
}

function setMoveEnable(status) {
    Ext.getCmp('move-prev').setDisabled(!status);
    Ext.getCmp('move-next').setDisabled(!status);
}
function GetProduct(frm) {
    Ext.Ajax.request({
        url: '/VendorProductCombo/QueryProduct',
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
function updateAuth(panel, str) {
    if (GetIsEdit == 'true' || GetCopyProductId() != '') {
        var cols = panel.query('*[' + str + ']');
        //Ext.Array.each(cols, function () { this.show(); });
    }
    else {
        setShow(panel, str);
    }
}

function Is_Continue(pro_id) {
    Ext.Msg.confirm(PROMPT, IS_CONTNUES, function (btn) {
        if (btn == 'no') {
            $(".x-tool-close").show();
            //刪除臨時表數據
            Ext.Ajax.request({
                url: '/VendorProductCombo/DeleteTempPro',
                method: 'post',
                params: {
                    OldProductId: GetCopyProductId(),
                    ProductId: pro_id
                }
            });
        }
        else {
            window.SetProductId(pro_id);
            window.frames[0].window.Page_Load();
        }
    });
    $(".x-tool-close").hide();
}


function rtnFrame(url) {
    return "<iframe scrolling='no' frameborder=0 width=100% height=100% src='" + url + "'></iframe>";
}
