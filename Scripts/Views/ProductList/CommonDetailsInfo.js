var tabs = new Array();
var product_status;
var create_channel;


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
        url: '/ProductList/ProductDetailsQuery',
        method: 'post',
        async: false,
        params: {
            "ProductId": product_id
        },
        success: function (response, opts) {
            var resText = Ext.decode(response.responseText);
            if (!resText) return;

            product_status = resText.data.product_status;
            create_channel = resText.data.create_channel;

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
                    title: '<div style="width:70px">' + PRODUCT_SPEC + '</div>',
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
                    title: '<div style="width:70px">' + PRODUCT_PRICE + '</div>',
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
                    title: '<div style="width:70px">' + STOCK + '</div>',
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
                    title: '<div style="width:70px">' + PICTURE + '</div>',
                    autoScroll: true,
                    items: [Pic, Pic2, other, expalinPic],//add by wwei0216w 2015/4/2 添加手機說明圖的Img
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
                    title: '<div style="width:70px">' + PRODUCT_SPEC + '</div>',
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
                    title: '<div style="width:70px">' + PRODUCT_PRICE + '</div>',
                    autoScroll: true,
                    items: [showPanel],
                    listeners: {
                        beforeactivate: function () {
                            siteProductStore.load({ params: { ProductId: product_id } });
                            Ext.Ajax.request({
                                url: '/Product/QueryProduct',
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
                                url: '/Product/GetPriceMaster',
                                method: 'post',
                                params: {
                                    "ProductId": product_id
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
                    title: '<div style="width:70px">' + STOCK + '</div>',
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
                    title: '<div style="width:70px">' + PICTURE + '</div>',
                    autoScroll: true,
                    items: [Pic, Pic2, other, standard, expalinPic],//add by wwei0216w 2015/4/2 添加手機說明圖的Img
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
                ToolAuthorityQueryByUrl("/ProductList/ProductDetails", function () { setShow(baseInfo, 'colName'); });
                Page_Load("baseInfo");
            }
        }
    });
    tabs.push(baseInfo);


    /******物流設定**********************************************************************/

    var transportSet = Ext.create('Ext.form.Panel', {
        id: 'transportSet',
        title: PHYSICAL_DISTRIBUTION_DISPATCH_MODE,
        border: false,
        autoScroll: true,
        layout: 'anchor',
        defaults: { anchor: "45%", msgTarget: "side", labelWidth: 100, margin: '8 0 15 8' },
        items: transportCheck
    });

    tabs.push(transportSet);


    /******描述**********************************************************************/
    var description = Ext.create('Ext.form.Panel', {
        title: '<div style="width:70px">' + DESCRIPTION + '</div>',
        id: 'description',
        autoScroll: true,
        layout: 'anchor',
        defaults: { anchor: "95%", msgTarget: "side", labelWidth: 130 },
        border: false,
        plain: true,
        bodyStyle: 'padding:5px 5px 20px 5px',
        items: descripItems,
        listeners: {
            beforeactivate: function () {
                //加載標籤
                Ext.Ajax.request({
                    url: '/ProductList/GetProTag',
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
                    url: '/ProductList/GetProNotice',
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


    /******課程**********************************************************************/
    initCoursePanel(product_id);
    var courseDetailView = Ext.create('Ext.form.Panel', {
        id: 'courseDetailView',
        title: '<div style="width:70px">' + CURRICULUM + '</div>',
        border: false,
        autoScroll: true,
        layout: 'anchor',
        defaults: { anchor: "45%", msgTarget: "side", labelWidth: 100, margin: '8 0 15 8' },
        items: [courseDetailItemGrid]
    });

    // tabs.push(courseDetailView);


    /********價格**********************************************************************/
    tabs.push(price);

    /*********類別**********************************************************************/
    initCatePanel(product_id);
    var category = Ext.create('Ext.panel.Panel', {
        title: '<div style="width:70px">' + PRODUCT_CATEGORY + '</div>',
        bodyStyle: 'padding:10px',
        autoScroll: true,
        items: [categoryPanel, stageCategoryPanel],
        listeners: {
            beforeactivate: function () {
                Ext.getCmp('stageCategoryPanel').getStore().load({ params: { ProductId: product_id } });
                category.clearListeners();
            },
            afterrender: function (panel) {
                setShow(panel, 'colName');
            }
        }
    });
    tabs.push(category);


    /*******新類別**********************************************************************/
    newinitCatePanel(product_id);
    var newcategory = Ext.create('Ext.panel.Panel', {
        title: '<div style="width:70px">' + NEW_PRODUCT_CATEGORY + '</div>',
        bodyStyle: 'padding:10px',
        autoScroll: true,
        items: [newcategoryPanel, newstageCategoryPanel],
        listeners: {
            beforeactivate: function () {
                newtreeStoreToo.load({ params: { ProductId: product_id } });
                newcategory.clearListeners();
            }
            ,
            afterrender: function (panel) {
                setShow(panel, 'colName');
            }
        }
    });
    tabs.push(newcategory);

    /********庫存**********************************************************************/
    tabs.push(stock);

    /*******圖檔*************************************************************************/
    tabs.push(picture);

    /*******抽獎**************************************************************************/
    initFortunePanel(product_id);
    var prize = Ext.create('Ext.panel.Panel', {
        title: '<div style="width:70px">' + FORTUNE + '</div>',
        bodyStyle: 'padding:10px',
        autoScroll: true,
        items: [fortunePanel],
        listeners: {
            afterrender: function (panel) {
                setShow(panel, 'colName');
            }
        }
    });
    tabs.push(prize);

    initProductStatusPanel(product_id, product_status, create_channel);
    var productStatus = Ext.create('Ext.panel.Panel', {
        title: '<div style="width:70px">' + STATUS + '</div>',
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
            url: '/ProductList/ProductDetailsQuery',
            actionMethods: 'post',
            params: {
                "ProductId": product_id
            },
            success: function (response, opts) {
                var resText = eval("(" + opts.response.responseText + ")");
                if (resText.data == null) return;
                if (resText.data.combination != "單一商品") {
                    Ext.getCmp("prepaid").hide();
                }
                //if (resText.data.prod_sz != "") {
                //    Ext.getCmp("product_name").setValue(resText.data.product_name + " (" + resText.data.prod_sz + ")");
                //}
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

                //add by Jiajun 2014/09/17

                if (resText.data.show_in_deliver == "") {
                    Ext.getCmp("show_in_deliver").setValue(NONE);
                }

                if (resText.data.show_in_deliver == "1") {
                    Ext.getCmp("show_in_deliver").setValue(YES);
                } else {
                    Ext.getCmp("show_in_deliver").setValue(NO);
                }

                if (resText.data.prepaid == "") {
                    Ext.getCmp("prepaid").setValue(NONE);
                }

                if (resText.data.prepaid == "1") {
                    Ext.getCmp("prepaid").setValue(YES);
                } else {
                    Ext.getCmp("prepaid").setValue(NO);
                }

                //add by Jiajun 2015/07/31
                if (resText.data.purchase_in_advance == "1") {
                    Ext.getCmp("purchase_in_advance").setValue(YES);
                    Ext.getCmp('purchase_in_advance_start').show();
                    Ext.getCmp('bl').show();
                    Ext.getCmp('purchase_in_advance_end').show();
                } else {
                    Ext.getCmp("purchase_in_advance").setValue(NO);
                    Ext.getCmp('purchase_in_advance_start').hide();
                    Ext.getCmp('bl').hide();
                    Ext.getCmp('purchase_in_advance_end').hide();
                }

                if (resText.data.purchase_in_advance_start != "0") {
                    var advance_start = Ext.Date.format(new Date(eval(resText.data.purchase_in_advance_start * 1000)), 'Y-m-d H:i:s');
                    Ext.getCmp("purchase_in_advance_start").setValue(advance_start);
                }

                if (resText.data.purchase_in_advance_end != "0") {
                    var advance_end = Ext.Date.format(new Date(eval(resText.data.purchase_in_advance_end * 1000)), 'Y-m-d H:i:s');
                    Ext.getCmp("purchase_in_advance_end").setValue(advance_end);
                }

                //add by Jiajun 2014/11/05
                if (product_id != "") {
                    Ext.Ajax.request({
                        url: '/Product/GetProductDeliverySet',
                        method: 'post',
                        async: false,
                        params: {
                            productId: product_id,
                            comboType: 0
                        },
                        success: function (data) {
                            var values = eval("(" + data.responseText + ")");
                            Ext.getCmp("transportCheck").setValue({
                                'transportGroup': values
                            });
                        }
                    });
                }
                //edit by Jiajun 2015.07.31
                if (product_id != "") {
                    Ext.Ajax.request({
                        url: '/ProductList/ParentListQuery',
                        method: 'post',
                        async: false,
                        params: {
                            productId: product_id
                        },
                        success: function (data) {
                            var values = eval("(" + data.responseText + ")");
                            if (values.data != "") {
                                Ext.getCmp("parent_list").setValue(values.data);
                            } else {
                                Ext.getCmp("parent_list").setValue(NONE);
                            }

                        }
                    });
                }

                ////add by zhuoqin0830w  2015/03/16  根據出貨方式 判斷 label 顯示 寄倉天數 或 調度天數 
                //if (resText.data.product_mode == "自出") {
                //    Ext.getCmp("extra_days").hide();
                //}
                //if (resText.data.product_mode == "寄倉") {
                //    $('#extra_days label').html(SEND_EXTRA_DAYS + ":");
                //} else if (resText.data.product_mode == "調度") {
                //    $('#extra_days label').html(CONTROL_EXTRA_DAYS + ":");
                //} else if (resText.data.product_mode == "自出") {
                //    Ext.getCmp("extra_days").hide();
                //}
            }
        })
        /**/
    }
}