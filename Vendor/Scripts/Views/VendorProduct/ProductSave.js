/* 
* Copyright (c) 2014，武漢聯綿信息技術有限公司鄭州分公司 
* All rights reserved. 
* 
* 文件名称：ProductSave.cs 
* 摘 要： 
* 單一商品新增 
* 当前版本：v1.0 
* 作 者： shuangshuang0420j 
* 完成日期： 
* 
*/

var CLICK_BTN_CHANGE = false; //存貯button狀態
Ext.onReady(function () {
    var tabs = new Array();
    // /******商品基本資料*************************************************************/ 
    var baseInfo = Ext.create('Ext.panel.Panel', {
        title: TITLE_BASE_INFO,
        html: rtnFrame('/VendorProduct/baseInfo'),
        listeners: {
            beforeactivate: function () {
                setTimeout(function () {
                    Ext.getCmp('move-prev').setDisabled(true);
                    Ext.getCmp('move-next').setDisabled(false);
                }, 2000); //{ setMoveEnable(true) }, 2000); 
            }
        }
    });
    tabs.push(baseInfo);


    /******描述**********************************************************************/
    var description = Ext.create('Ext.panel.Panel', {
        title: TITLE_DESCRIPTION,
        html: window.top.rtnFrame('/VendorProduct/Description'),
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
        html: window.top.rtnFrame('/VendorProduct/SpecIndex'),
        listeners: {
            beforeactivate: function () {
                spec.update(window.top.rtnFrame('/VendorProduct/SpecIndex'));
                setTimeout(function () { setMoveEnable(true) }, 2000);
            }
        }
    });
    tabs.push(spec);

    /********價格**********************************************************************/
    var price = Ext.create('Ext.panel.Panel', {
        title: TITLE_PRICE,
        //html: window.top.rtnFrame('/Product/Price'), 
        listeners: {
            beforeactivate: function () {
                price.update(window.top.rtnFrame('/VendorProduct/PriceIndex'));
                setTimeout(function () { setMoveEnable(true) }, 2000);
            }
        }
    });
    tabs.push(price);

    // /*********類別**********************************************************************/ 
    var category = Ext.create('Ext.panel.Panel', {
        title: TITLE_CATEGORY,
        html: window.top.rtnFrame('/VendorProduct/ProductCategory'),
        listeners: {
            beforeactivate: function () {
                setTimeout(function () { setMoveEnable(true) }, 2000);
            }
        }
    });

    tabs.push(category);
    /********庫存**********************************************************************/
    var stock = Ext.create('Ext.panel.Panel', {
        title: TITLE_STOCK,
        //html: window.top.rtnFrame('/Product/productStock'), 
        listeners: {
            beforeactivate: function () {
                stock.update(window.top.rtnFrame('/VendorProduct/productStock'));
                setTimeout(function () { setMoveEnable(true) }, 2000);
                if (GetIsEdit() == "false") {//新增時 
                    Ext.getCmp('btnTempSave').show();
                }
            }
        }
    });
    tabs.push(stock);
    // /*******圖檔*************************************************************************/ 
    var picture = Ext.create('Ext.panel.Panel', {
        title: TITLE_PICTURE,
        //html: window.top.rtnFrame('/Product/productPic'), 
        listeners: {
            beforeactivate: function () {
                picture.update(window.top.rtnFrame('/VendorProduct/productPic'));
                setTimeout(function () {
                    Ext.getCmp('move-prev').setDisabled(false);
                    Ext.getCmp('move-next').setDisabled(true);
                }, 2000); // { setMoveEnable(true) }, 2000); 
                if (GetIsEdit() == "false") {//獲取不到pid時為新增，顯示儲存，將數據加入product_temp 
                    Ext.getCmp('btnTempSave').show();
                }
            }
        }
    });
    tabs.push(picture);

    var saveForm = Ext.create('Ext.tab.Panel', {
        id: 'ContentPanel',
        minTabWidth: 100,
        tabPosition: 'bottom',
        activeTab: 0,
        title: '&nbsp;',
        layout: 'fit',
        items: [tabs],
        frame: true,
        buttons: [
                { xtype: 'component', height: 25 },
                {
                    text: BTN_TEMP_SAVE,
                    id: 'btnTempSave',
                    iconCls: 'icon-add',
                    hidden: GetIsEdit() == "false" ? true : false,
                    handler: function () {
                        if (GetIsEdit() == "true") {
                            saveForm.getActiveTab().body.dom.firstChild.contentWindow.save();
                        }
                        else {
                            saveForm.getActiveTab().body.dom.firstChild.contentWindow.saveTemp();
                        }
                    }
                }
                ],
        tbar: [
               {
                   id: 'move-prev',
                   iconCls: 'icon-prev',
                   text: PERV_MOVE,
                   handler: function () {
                       CLICK_BTN_CHANGE = true;
                       var panel = Ext.getCmp('ContentPanel');
                       var t = panel.getActiveTab().prev(); //獲得本面板的前一個面板 
                       if (t) {
                           panel.setActiveTab(t); //若存在設置前一個面板為活動面板 
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
                 text: NEXT_MOVE,
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
                if (GetIsEdit() == "false") {//新增時每步一保存 
                    if (!CLICK_BTN_CHANGE) { return false; }
                    setMoveEnable(false);
                    if (newCard.title != TITLE_STOCK && newCard.title != TITLE_PICTURE && newCard.title != TITLE_PRIZE) {
                        Ext.getCmp('btnTempSave').hide(); //當庫存、價格、圖檔時會顯示儲存 
                    }
                    if (!oldCard.body.dom.firstChild.contentWindow.save()) {//舊的tab保存成功時才會跳轉頁面 
                        setMoveEnable(true);
                        return false;
                    }
                }
                else {
                    if (newCard.title != TITLE_PRICE) {
                        Ext.getCmp('btnTempSave').show();
                    }
                    else {
                        Ext.getCmp('btnTempSave').hide();
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
                    QueryToolAuthorityByUrl('/VendorProduct/ProductSave');
                }
            }
        }
    });
})
//獲取編輯時的pid 
function GetProductId() {
    return document.getElementById('ProductId').value;
}
function SetProductId(value) {
    var retBool = true;
    document.getElementById('ProductId').value = value;
    return retBool;
}

function GetIsEdit() {//true：修改；false：新增 
    return document.getElementById('isEdit').value;
}
//控制上一步和下一步的顯示問題 
function setMoveEnable(status) {
    Ext.getCmp('move-prev').setDisabled(!status);
    Ext.getCmp('move-next').setDisabled(!status);
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
//新增時無權限控制 
function updateAuth(panel, str) {
    if (GetProductId() == '' || GetCopyProductId() != '') {
        var cols = panel.query('*[' + str + ']');
        // Ext.Array.each(cols, function () { this.show(); });
    }
    else {
        setShow(panel, str);
    }
}



function GetProduct(frm) {
    Ext.Ajax.request({
        url: '/VendorProduct/QueryProduct',
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
//提示是否繼續新增，繼續則加載數據，否則刪除該條臨時數據 
function Is_Continue(prod_id) {
    Ext.Msg.confirm(PROMPT, IS_CONTNUES, function (btn) {
        if (btn == 'no') {
            $(".x-tool-close").show();
            //刪除臨時表數據 
            Ext.Ajax.request({
                url: '/VendorProduct/DeleteTempPro',
                method: 'post',
                params: {
                    ProductId: prod_id,
                    OldProductId: GetCopyProductId()

                }
            });
        }
        else {
            window.SetProductId(prod_id);
            window.frames[0].window.Page_Load();
        }
    });
    $(".x-tool-close").hide();
}

function rtnFrame(url) {
    return "<iframe scrolling='no' frameborder=0 width=100% height=100% src='" + url + "'></iframe>";
} 
