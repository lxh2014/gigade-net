var tabs = new Array();
var product_status;

function rtnFrame(url) {
    return "<iframe id='if_channel' frameborder=0 width=100% height=100% src='" + url + "' ></iframe>";
}

function createForm(product_id) {

    var price;
    var stock;
    var picture;
    var spec;

    //判斷商品類型 (單一商品 OR 組合商品)
    Ext.Ajax.request({
        url: '/ProductVendorList/ProductDetailsQuery',
        method: 'post',
        async: false,
        params: {
            "ProductId": product_id
        },
        success: function (response, opts) {
            var resText = Ext.decode(response.responseText);
            if (!resText) return;

            product_status = resText.data.product_status;

            if (resText.data.product_type != 1 && resText.data.product_type != 0) {//組合商品
                /************重畫介面****************************************/
                //重畫組合商品價格介面
                createPricePanel(resText.data.product_type, product_id, resText.data.price_type);
                //重畫組合商品庫存介面
                comboStockDetails(resText.data.product_type, product_id);
                //重畫圖檔
                PicPanel(product_id);

                /***************創建介面*************************************/

                //創建組合商品規格介面
                initComboSpecPanel(product_id);

                spec = Ext.create('Ext.panel.Panel', {
                    title: '<div style="width:85px">' + PRODUCT_SPEC + '</div>',
                    id: 'tabComboSpecPanel',
                    bodyStyle: 'padding:10px',
                    autoScroll: true,
                    items: [specPanel],
                    listeners: {
                        afterrender: function () {
                            setShow(spec, 'colName');
                        }
                    }
                });
                //創建組合商品價格介面
                price = Ext.create('Ext.form.Panel', {
                    title: '<div style="width:85px">' + PRODUCT_PRICE + '</div>',
                    autoScroll: true,
                    items: [firPanel, showComboPanel],
                    listeners: {
                        beforeactivate: function () {
                            PriceLoadTogether(resText.data.product_type, product_id);
                            price.clearListeners();
                        },
                        afterrender: function () {
                            setShow(price, 'colName');
                        }
                    }
                });
                //創建組合商品庫存介面
                stock = Ext.create('Ext.form.Panel', {
                    title: '<div style="width:85px">' + STOCK + '</div>',
                    autoScroll: true,
                    items: [chekPanel, stockPanel],
                    listeners: {
                        afterrender: function () {
                            setShow(stock, 'colName');
                        }
                    }
                });
                //創建組合商品圖檔介面
                picture = Ext.create('Ext.panel.Panel', {
                    title: '<div style="width:85px">' + PICTURE + '</div>',
                    autoScroll: true,
                    items: [Pic, other, expalinPic],
                    listeners: {
                        beforeactivate: function () {
                            PicLoadTogether(product_id);
                            picture.clearListeners();
                        },
                        afterrender: function () {
                            setShow(picture, 'colName');
                        }
                    }
                });
                /********************end***************************************/
            }
            else { //單一商品
                /************重畫介面****************************************/
                //重畫單一商品庫存介面
                singleStockDetails(resText.data.product_type, product_id);
                //重畫圖檔
                PicPanel(product_id);
                //重畫價格
                singlePricePanel(product_id);

                /***************創建介面*************************************/

                //創建單一商品規格介面
                initSpecPanel(product_id);

                spec = Ext.create('Ext.panel.Panel', {
                    title: '<div style="width:85px">' + PRODUCT_SPEC + '</div>',
                    bodyStyle: 'padding:10px',
                    autoScroll: true,
                    items: [specPanel],
                    listeners: {
                        afterrender: function () {
                            setShow(spec, 'colName');
                        }
                    }
                });


                //創建單一商品價格介面
                price = Ext.create('Ext.form.Panel', {
                    title: '<div style="width:85px">' + PRODUCT_PRICE + '</div>',
                    autoScroll: true,
                    items: [showPanel],
                    listeners: {
                        beforeactivate: function () {
                            siteProductStore.load({ params: { ProductId: product_id, "IsEdit": "true"} });
                            Ext.Ajax.request({
                                url: '/ProductVendorList/QueryProduct',
                                method: 'post',
                                params: {
                                    "ProductId": product_id
                                },
                                success: function (response) {
                                    var reStr = Ext.decode(response.responseText);
                                    if (reStr.length != 0) {
                                        Ext.getCmp("product_price_list").setValue(reStr.data.Product_Price_List);
                                        Ext.getCmp("bag_check_money1").setValue(reStr.data.Bag_Check_Money);
                                        Ext.getCmp("show_pricelist2").setValue(reStr.data.show_listprice);
                                    }
                                }
                            });
                            Ext.Ajax.request({
                                url: '/ProductVendorList/GetPriceMaster',
                                method: 'post',
                                params: {
                                    "ProductId": product_id,
                                    "IsEdit": "true"//標誌方法為修改

                                },
                                success: function (form, action) {
                                    var result = Ext.decode(form.responseText);
                                    if (result) {
                                        for (var i = 0; i < result.length; i++) {
                                            if (result[i].site_id == 1 && result[i].user_level == 1 && result[i].user_id == 0) {
                                                Ext.getCmp("same_price").setValue(result[i].same_price); break;
                                            }
                                        }

                                    }

                                }
                            });
                            price.clearListeners();
                        },
                        afterrender: function (panel) {
                            setShow(panel, 'colName');
                        }
                    }
                });
                //創建單一商品庫存介面
                stock = Ext.create('Ext.form.Panel', {
                    title: '<div style="width:85px">' + STOCK + '</div>',
                    autoScroll: true,
                    items: [chekPanel, stockPanel],
                    listeners: {
                        afterrender: function () {
                            setShow(stock, 'colName');
                        }
                    }

                });

                //創建單一商品圖檔介面
                picture = Ext.create('Ext.panel.Panel', {
                    title: '<div style="width:85px">' + PICTURE + '</div>',
                    autoScroll: true,
                    items: [Pic, other, standard, expalinPic],
                    listeners: {
                        beforeactivate: function () {
                            PicLoadTogether(product_id);
                            picture.clearListeners();
                        },
                        afterrender: function () {
                            setShow(picture, 'colName');
                        }
                    }

                });
                /********************end***************************************/
            }

        }
    });


    /********************商品基本資料**********************/

    var baseInfo = Ext.create('Ext.form.Panel', {
        id: 'baseInfo',
        title: BASEINFO,
        border: false,
        autoScroll: true,
        layout: 'anchor',
        defaults: { anchor: "95%", msgTarget: "side", labelWidth: 100, margin: '8 0 15 8' },
        items: baseItems,
        listeners: {
            beforerender: function () {
                ToolAuthorityQueryByUrl("/ProductVendorList/ProductDetails", function () { setShow(baseInfo, 'colName'); });
                Page_Load("baseInfo");
            }
        }
    });
    tabs.push(baseInfo);

    /******描述**********************************************************************/
    var description = Ext.create('Ext.form.Panel', {
        title: '<div style="width:85px">' + DESCRIPTION + '</div>',
        id: 'description',
        autoScroll: true,
        layout: 'anchor',
        defaults: { anchor: "95%", msgTarget: "side", labelWidth: 100 },
        border: false,
        plain: true,
        bodyStyle: 'padding:5px 5px 20px 5px',
        items: descripItems,
        listeners: {
            beforeactivate: function () {
                //加載標籤
                Ext.Ajax.request({
                    url: '/ProductVendorList/GetProTag',
                    method: 'post',
                    params: {
                        ProductId: product_id
                    },
                    success: function (form, action) {
                        Ext.getCmp('product_tag_set').update(form.responseText);
                    },
                    failure: function () {
                        Ext.Msg.alert(INFORMATION, DATA_LOAD_FAILURE);
                        return false;
                    }
                })

                //加載公告
                Ext.Ajax.request({
                    url: '/ProductVendorList/GetProNotice',
                    method: 'post',
                    params: {
                        ProductId: product_id
                    },
                    success: function (form, action) {
                        Ext.getCmp('product_notice_set').update(form.responseText);
                    },
                    failure: function () {
                        Ext.Msg.alert(INFORMATION, DATA_LOAD_FAILURE);
                        return false;
                    }
                });
                description.clearListeners();
            },
            beforerender: function () {
                setShow(description, 'colName');
                Page_Load("description");
            }
        }
    });
    tabs.push(description);

    /*******規格*********************************************************************/
    tabs.push(spec);

    /********價格**********************************************************************/
    tabs.push(price);

    /*********類別**********************************************************************/
    initCatePanel(product_id);
    var category = Ext.create('Ext.panel.Panel', {
        title: '<div style="width:85px">' + PRODUCT_CATEGORY + '</div>',
        bodyStyle: 'padding:10px',
        autoScroll: true,
        items: [categoryPanel, stageCategoryPanel],
        listeners: {
            beforeactivate: function () {
                Ext.getCmp('stageCategoryPanel').getStore().load({ params: { ProductId: product_id} });
                category.clearListeners();
            },
            afterrender: function (panel) {
                setShow(panel, 'colName');
            }
        }
    });
    tabs.push(category);

    /********庫存***********/
    tabs.push(stock);

    /*******圖檔************/
    tabs.push(picture);

    /*******狀態***********/
    initProductStatusPanel(product_id, product_status);
    var productStatus = Ext.create('Ext.panel.Panel', {
        title: '<div style="width:85px">' + STATUS + '</div>',
        bodyStyle: 'padding:10px',
        autoScroll: true,
        items: [currentProductstatusPanel, productStatusPanel],
        listeners: {
            afterrender: function (panel) {
                setShow(panel, 'colName');
            }
        }
    });
    tabs.push(productStatus);



    //加載頁面
    function Page_Load(panel) {
        /**/
        Ext.getCmp(panel).getForm().load({
            type: 'ajax',
            url: '/ProductVendorList/ProductDetailsQuery',
            actionMethods: 'post',
            params: {
                "ProductId": product_id
            },
            success: function (response, opts) {
                var resText = eval("(" + opts.response.responseText + ")");
                if (resText.data == null) return;
                if (resText.data.product_start != "0") {
                    var date_start = Ext.Date.format(new Date(eval(resText.data.product_start * 1000)), 'Y-m-d H:i:s');
                    Ext.getCmp("product_start").setValue(date_start);
                }
                else {
                    Ext.getCmp("product_start").setValue(NONE);
                }

                if (resText.data.product_end != "0") {
                    var date_end = Ext.Date.format(new Date(eval(resText.data.product_end * 1000)), 'Y-m-d H:i:s');
                    Ext.getCmp("product_end").setValue(date_end);
                }
                else {
                    Ext.getCmp("product_end").setValue(NONE);
                }

                if (resText.data.expect_time != "0") {
                    var expect_time = Ext.Date.format(new Date(eval(resText.data.expect_time * 1000)), 'Y-m-d H:i:s');
                    Ext.getCmp("expect_time").setValue(expect_time);
                } else {
                    Ext.getCmp("expect_time").setValue(NONE);
                }
                if (resText.data.product_vendor_code == "") {
                    Ext.getCmp("product_vendor_code").setValue(NONE);
                }
                if (resText.data.expect_msg == "") {
                    Ext.getCmp("expect_msg").setValue(NONE);
                }

                if (resText.data.tax_type == "1") {
                    Ext.getCmp("tax_type").setValue(TAX_NEED);
                } else {
                    Ext.getCmp("tax_type").setValue(TAX_NO);
                }
            }
        })

    }

}

