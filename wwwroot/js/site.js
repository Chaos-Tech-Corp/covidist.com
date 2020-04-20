var _charts = {}, _countryName = 'World';

function loadData(filter, range, adjust) {
    var continent = $("#bnFilterCountry").val();
    $("#lyr-loading").show();
//    $.post('/AllData', { field: filter, range: range, adjust: adjust }, function (result) {
    $.post(continent == 'World' ? '/AllData' : '/ContinentData', { continent: continent.substring(2), field: filter, range: range, adjust: adjust }, function (result) {
        _charts['lyrWorld'] = Highcharts.chart('lyrWorld', {
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

function loadBar(filter, sort, dir) {
    $("#lyr-loading").show();
    $.post('/BarData', { filter: filter, sort: sort, dir:dir }, function (result) {
        _charts['lyrDeathMillion'] = Highcharts.chart('lyrDeathMillion', {
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
                text: null// 'Deaths per Million Population'
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

function loadCountry(lyr, t) {
    $("#lyr-loading").show();
    $.post('/mobile_CountryData', { country: _countryName, type: t, s: $("#bnTransmissibility").val() }, function (result) {
        //check if the series contains an estimate
        var plotBands = [],
            has2Axis = false,
            yAxisConfig = [];

        for (var i = 0; i < result.series.length; i++) {
            //if (result.series[i].name == 'Estimate Infected') {
            //    plotBands = plotBands.length > 0 ? plotBands : [{
            //        label: {
            //            text: 'Estimate'
            //        },
            //        color: '#f0f0f0',
            //        from: result.series[i].data[0][0] - 56400000, // Start of the plot band
            //        to: result.series[i].data[result.series[i].data.length - 1][0] + 76400000 // End of the plot band
            //    }];
            //}
            if (result.series[i].yAxis > 0) {
                has2Axis = true;
            }
        };

        var yAxis0 = {
            title: {
                text: 'Total Cases'
            },
            type: 'linear',
            plotLines: result.yLines
        };
        yAxis1 = {
            title: {
                text: t =='m' ? '' : 'Daily Cases'
            }, min: 0,
            labels: {
                style: {
                    color: Highcharts.getOptions().colors[0]
                }
            },
            opposite: true
        };
        yAxisConfig.push(yAxis0);
        if (has2Axis) {
            yAxisConfig.push(yAxis1);
        }

        _charts[lyr] = Highcharts.chart(lyr, {
            chart: {
                zoomType: 'xy'
            },
            title: false,
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
                layout: 'horizontal',
                align: 'center',
                verticalAlign: 'bottom'
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

$(function () {
    _countryName = $("#bnFilterCountry").val();

    $("#bnFilterCountry").on("change", function () {
        _countryName = $(this).val();
        let isContinent = _countryName[0] == 'c' && _countryName[1] == '-';
        if (isContinent) {
            document.location = '/mobile/' + _countryName.substring(2);
        } else {
            document.location = '/mobile/' + _countryName;
        }
    });

    let isContinent = _countryName[0] == 'c' && _countryName[1] == '-';

    if (_countryName == 'World' || isContinent) {

        $("#bnFilterAdjust").closest(".form_element").hide();
        
        $("#bnFilterDate").on("change", function () {
            var val = $("#bnFilterDate").val();
            if (val == "date") {
                $("#bnFilterAdjust").closest(".form_element").hide();
            } else {
                $("#bnFilterAdjust").closest(".form_element").show();
            }
            loadData($("#bnFilterLine").val(), val, $("#bnFilterAdjust").val());
        });

        $("#bnFilterAdjust, #bnFilterLine").on("change", function () {
            loadData($("#bnFilterLine").val(), $("#bnFilterDate").val(), $("#bnFilterAdjust").val());
        });

        $(".scale-type").on("change", function () {
            var t = $(this),
                lyr = t.closest('section').find('.chart-container').attr('id');
            if (t.val() == null || t.val() == '') {
                return;
            }
            _charts[lyr].update({
                yAxis: [{
                    type: t.val()
                }]
            })
        });

        $("#bnSortField,#bnSortDirection").on("change", function () {
            loadBar('million', $("#bnSortField").val(), $("#bnSortDirection").val());
        });

        loadData($("#bnFilterLine").val(), $("#bnFilterDate").val(), $("#bnFilterAdjust").val());
        loadBar('million');

    } else {

        $("#bnTransmissibility").val('3.5');
        loadCountry('crtTotalCases', 'c');
        loadCountry('crtActiveCases', 'a');
        loadCountry('crtTotalDeath', 'd');
        loadCountry('crtEstimation', 'e');
        loadCountry('crtLinearEstimation', 'l');
        loadCountry('crtMobility', 'm');

        $(".scale-type").on("change", function () {
            var t = $(this),
                lyr = t.closest('section').find('.chart-container').attr('id');
            if (t.val() == null || t.val() == '') {
                return;
            }
            _charts[lyr].update({
                yAxis: [{
                    type: t.val()
                }]
            });
        });
        $("#bnTransmissibility").on("change", function () {
            loadCountry('crtEstimation', 'e');
        });
    }

    $("select").selectpicker();
});