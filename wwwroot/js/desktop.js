var chart = null,
    vSeries = [],
    _hash = null,
    _isDesktop = window.location.href.toLowerCase().indexOf('desktop') > 0;

function loadData(filter, range, adjust) {
    $("#lyr-loading").show();
    if (chart != null) {
        for (var i = 0; i < chart.series.length; i++) {
            vSeries[chart.series[i].name] = chart.series[i].visible;
        }
    }

    if (filter == "million") {
        return loadBar(filter);
    }

    $.post('/AllData', { field: filter, range: range, adjust: adjust }, function (result) {
        if (chart != null) {
            for (var i = 0; i < result.length; i++) {
                var vValue = vSeries[result[i].name];
                result[i].visible = vValue === undefined ? 'false' : vValue;
            }
        }

        chart = Highcharts.chart('container', {
            chart: {
                type: 'spline'
            },
            //boost: {
            //    enabled: false,
            //    usePreallocated: true,
            //    useAlpha: false
            //},
            title: {
                text: null
            },
            xAxis: {
                type: range == 'pandemic' ? 'linear' : 'datetime',
                dateTimeLabelFormats: {
                    month: '%e. %b',
                    year: '%b'
                },
                title: {
                    text: 'Date'
                }
            },
            yAxis: [{
                title: {
                    text: 'Cases'
                }
                , type: $("#bnFilterScale").val()
                //, min: 0
            }],

            legend: {
                layout: 'vertical',
                align: 'right',
                verticalAlign: 'middle'
            },

            tooltip: {
                headerFormat: '<b>{series.name}</b><br>',
                pointFormat: range == 'pandemic' ? 'Day {point.x}: {point.y:#,##0f}' : '{point.x:%e. %b}: {point.y:#,##0f}'
            },

            plotOptions: {
                series: {
                    marker: {
                        enabled: false
                    }
                }
            },

            series: result,

            responsive: {
                rules: [{
                    condition: {
                        maxWidth: 500
                    },
                    chartOptions: {
                        legend: {
                            layout: 'horizontal',
                            align: 'center',
                            verticalAlign: 'bottom'
                        }
                    }
                }]
            }

        });

        $("#lyr-loading").fadeOut();
    });
}

function loadCountry() {

    $("#lyr-loading").show();
    var filter = $("#bnFilterType").val(),
        cntry = $("#bnFilterCountry").val(),
        s = $("#bnTransmissibility").val();

    if (filter == "e") {
        $("#bnTransmissibility").closest(".form_element").show();
    } else {
        $("#bnTransmissibility").closest(".form_element").hide();
    }

    $.post('/CountryData', { country: cntry, type: filter, s: s }, function (result) {
        //check if the series contains an estimate
        var plotBands = [],
            has2Axis = false,
            yAxisConfig = [];
        for (var i = 0; i < result.series.length; i++) {
            if (result.series[i].name == 'Estimate Infected') {
                plotBands = plotBands.length > 0 ? plotBands : [{
                    label: {
                        text: 'Estimate'
                    },
                    color: '#f0f0f0',
                    from: result.series[i].data[0][0] - 56400000, // Start of the plot band
                    to: result.series[i].data[result.series[i].data.length - 1][0] + 76400000 // End of the plot band
                }];
            }
            if (result.series[i].yAxis > 0) {
                has2Axis = true;
            }
        };
        var yAxis0 = {
            title: {
                text: 'Cases'
            },
            type: $("#bnFilterScale").val()
            //, min: 0
        };
        yAxis1 = {
            title: {
                text: null
            }, min: 0,
            labels: {
                style: {
                    color: Highcharts.getOptions().colors[0]
                }
            },
            opposite: true
        };
        if (filter == 'c') {
            yAxis1.plotLines = [{
                color: Highcharts.getOptions().colors[0],
                width: 1,
                value: 1,
                label: {
                    text: 'No more new cases'
                }
            }];
        }
        yAxisConfig.push(yAxis0);
        if (has2Axis) {
            yAxisConfig.push(yAxis1);
        }

        chart = Highcharts.chart('container', {
            chart: {
                zoomType: 'xy'
            },
            title: {
                text: null
            },
            xAxis: {
                type: 'datetime',
                dateTimeLabelFormats: { // don't display the dummy year
                    month: '%e. %b',
                    year: '%b'
                },
                title: {
                    text: 'Date'
                },
                plotBands: plotBands,
                plotLines: result.lines
            },
            yAxis: yAxisConfig,
            legend: {
                layout: 'vertical',
                align: 'right',
                verticalAlign: 'middle'
            },

            tooltip: {
                headerFormat: '<b>{series.name}</b><br>',
                pointFormat: '{point.x:%e. %b}: {point.y:#,##0f}'
            },


            series: result.series,

            responsive: {
                rules: [{
                    condition: {
                        maxWidth: 500
                    },
                    chartOptions: {
                        legend: {
                            layout: 'horizontal',
                            align: 'center',
                            verticalAlign: 'bottom'
                        }
                    }
                }]
            }

        });
        $("#lyr-loading").fadeOut();
    });
}

function loadBar(filter) {
    $.post('/BarData', { filter: filter }, function (result) {
        if (chart != null) {
            for (var i = 0; i < result.length; i++) {
                var vValue = vSeries[result[i].name];
                result[i].visible = vValue === undefined ? 'false' : vValue;
            }
        }

        chart = Highcharts.chart('container', {
            chart: {
                type: 'bar',
                scrollablePlotArea: {
                    minHeight: (result.categories.length * 16)
                },
                marginRight: 32
            },
            //boost: {
            //    enabled: false,
            //    usePreallocated: true,
            //    useAlpha: false
            //},
            title: {
                text: 'Deaths per Million Population'
            },
            xAxis: {
                categories: result.categories,
                title: {
                    text: 'Country'
                }
            },
            yAxis: {
                min: 0,
                title: {
                    text: 'Cases',
                    align: 'high'
                },
                labels: {
                    overflow: 'justify'
                }
            },

            legend: {
                enabled: false
            },

            plotOptions: {
                bar: {
                    dataLabels: {
                        enabled: true
                    }
                }
            },

            series: [result.series],


        });
        $("#lyr-loading").fadeOut();
    });
}

function isMobile() {
    try {
        document.createEvent("TouchEvent"); return screen.width < 1024;
    } catch (e) { return false; }
}

if (isMobile() && !_isDesktop) {
    var hash = (window.location.hash != null && window.location.hash != '') ? '/' + window.location.hash.substring(1) : '';
    document.location = '/mobile' + hash;
}

$(function () {

    $.post('/getCountries', {}, function (result) {
        var s = $("#sel-countries");
        for (var i = 0; i < result.length; i++) {
            s.append('<option>' + result[i] + '</option>');
        }
        $("select").selectpicker();

        if (window.location.hash) {
            _hash = window.location.hash.substring(1);
            $("#bnFilterCountry").val(_hash).trigger("change");
        } else {
            loadData('infected', 'date', 100);
        }

        $("#lyr-loading").fadeOut();
    });

    $("#bnFilterAdjust").closest(".form_element").hide();
    $("#bnFilterType").closest(".form_element").hide();
    $("#bnTransmissibility").val("3.5").closest(".form_element").hide();

    $("#bnLegend").on("click", function () {
        chart.update({
            legend: {
                enabled: !chart.legend.display
            }
        });
    });

    $("#bnHideAll").on("click", function () {
        const hideAll = function (i) {
            window.setTimeout(function () {
                if (i < chart.series.length) {
                    chart.series[i].hide();
                    hideAll(++i);
                }
            }, 10);
        }
        hideAll(0);
    });
    $("#bnShowAll").on("click", function () {
        const showAll = function (i) {
            window.setTimeout(function () {
                if (i < chart.series.length) {
                    chart.series[i].show();
                    showAll(++i);
                }
            }, 10);
        }
        showAll(0);
    });

    $("#bnFilterLine").on("change", function () {
        loadData($(this).val(), $("#bnFilterDate").val());
    });

    $("#bnFilterDate").on("change", function () {
        var val = $("#bnFilterDate").val();
        if (val == "date") {
            $("#bnFilterAdjust").closest(".form_element").hide();
        } else {
            $("#bnFilterAdjust").closest(".form_element").show();
        }
        loadData($("#bnFilterLine").val(), val, $("#bnFilterAdjust").val());
    });

    $("#bnFilterAdjust").on("change", function () {
        loadData($("#bnFilterLine").val(), $("#bnFilterDate").val(), $("#bnFilterAdjust").val());
    });

    $("#bnFilterType,#bnTransmissibility").on("change", function () {
        loadCountry();
    });

    $("#bnFilterScale").on("change", function () {
        var type = $(this).val();
        chart.update({
            yAxis: [{
                type: type
            }, {}]
        });
    });

    $("#bnFilterCountry").on("change", function () {
        _hash = $(this).val();
        //window.location.hash = _hash;
        if (history.pushState) {
            history.pushState(null, null, "#" + _hash);
        }
        else {
            location.hash = "#" + _hash;
        }
        if (_hash == "World") {
            $("#bnFilterLine").closest(".form_element").show();
            $("#bnFilterDate").closest(".form_element").show();
            if ($("#bnFilterDate").val() == 'date') {
                $("#bnFilterAdjust").closest(".form_element").hide();
            } else {
                $("#bnFilterAdjust").closest(".form_element").show();
            }
            $("#bnFilterType, #bnTransmissibility").closest(".form_element").hide();
            loadData($("#bnFilterLine").val(), $("#bnFilterDate").val(), $("#bnFilterAdjust").val());
        } else {
            $("#bnFilterLine").closest(".form_element").hide();
            $("#bnFilterDate").closest(".form_element").hide();
            $("#bnFilterAdjust").closest(".form_element").hide();
            $("#bnFilterType, #bnTransmissibility").closest(".form_element").show();
            loadCountry();
        }
    });

    $("#bnOpenInfoWindow").on("click", function () {
        $("#infoWindow").css('display', 'flex');
    });

    $("#bnCloseInfoWindow").on("click", function () {
        $("#infoWindow").css('display', 'none');
    });


});

window.onhashchange = function () {
    if (window.location.hash != null && window.location.hash != '') {
        var hash = window.location.hash.substring(1);
        if (hash != _hash) {
            _hash = hash;
            $("#bnFilterCountry").val(_hash).trigger("change");
        }
    }
}