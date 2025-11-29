import React from 'react';
import {
    LineChart,
    Line,
    XAxis,
    YAxis,
    CartesianGrid,
    Tooltip,
    Legend,
    ResponsiveContainer,
    ReferenceLine,
    ReferenceArea,
} from 'recharts';

const DeviceChart = ({ data, timeRange, deviceType }) => {
    // Get safe ranges theo device type
    const getSafeRanges = (type) => {
        switch (type) {
            case 0: // Cooler
                return { min: 2, max: 10 };
            case 1: // Freezer
                return { min: -20, max: -5 };
            case 2: // Dehumidifier
                return { min: 15, max: 25 };
            default:
                return { min: 0, max: 30 };
        }
    };

    const safeRange = getSafeRanges(deviceType);

    // Xác định status của nhiệt độ (normal, warning, danger)
    const getTemperatureStatus = (temp) => {
        if (temp === null || temp === undefined || isNaN(temp)) return 'normal';

        switch (deviceType) {
            case 0: // Cooler
                if (temp >= 2 && temp <= 10) return 'normal';
                if ((temp >= 0 && temp < 2) || (temp > 10 && temp <= 15)) return 'warning';
                return 'danger';
            case 1: // Freezer
                if (temp >= -20 && temp <= -5) return 'normal';
                if ((temp >= -25 && temp < -20) || (temp > -5 && temp <= 0)) return 'warning';
                return 'danger';
            case 2: // Dehumidifier
                if (temp >= 15 && temp <= 25) return 'normal';
                if ((temp >= 10 && temp < 15) || (temp > 25 && temp <= 30)) return 'warning';
                return 'danger';
            default:
                return 'normal';
        }
    };

    // Format number với 2 chữ số sau dấu phẩy
    const formatNumber = (value) => {
        if (value === null || value === undefined || isNaN(value)) return '0.00';
        return Number(value).toFixed(2);
    };

    // Format time label based on range
    function formatTimeLabel(date, range) {
        switch (range) {
            case '1h':
            case '6h':
                return date.toLocaleTimeString('vi-VN', { hour: '2-digit', minute: '2-digit' });
            case '24h':
                return date.toLocaleTimeString('vi-VN', { hour: '2-digit' });
            case '7d':
                return date.toLocaleDateString('vi-VN', { month: 'short', day: 'numeric', hour: '2-digit' });
            case '30d':
                return date.toLocaleDateString('vi-VN', { month: 'short', day: 'numeric' });
            default:
                return date.toLocaleString('vi-VN');
        }
    }

    // Transform data for chart và thêm status cho mỗi điểm
    const chartData = data.map((item) => {
        const temp = item.temperature?.avg || 0;
        const timestamp = new Date(item.timestamp);
        return {
            timestamp: timestamp.getTime(),
            timeLabel: formatTimeLabel(timestamp, timeRange),
            temperature: temp,
            tempStatus: getTemperatureStatus(temp), // Thêm status để dùng cho custom dot
            tempMin: item.temperature?.min || 0,
            tempMax: item.temperature?.max || 0,
            humidity: item.humidity?.avg || 0,
            humMin: item.humidity?.min || 0,
            humMax: item.humidity?.max || 0,
        };
    });

    // Debug: Log chart data
    console.log('DeviceChart - timeRange:', timeRange, 'data points:', chartData.length);
    if (chartData.length > 0) {
        console.log('First point:', chartData[0]);
        console.log('Last point:', chartData[chartData.length - 1]);
    }

    // Custom tooltip
    const CustomTooltip = ({ active, payload, label }) => {
        if (active && payload && payload.length) {
            return (
                <div
                    style={{
                        backgroundColor: 'white',
                        border: '1px solid #ccc',
                        borderRadius: '4px',
                        padding: '10px',
                        boxShadow: '0 2px 4px rgba(0,0,0,0.1)',
                    }}
                >
                    <p style={{ margin: 0, fontWeight: 'bold', marginBottom: '5px' }}>
                        {label}
                    </p>
                    {payload.map((entry, index) => (
                        <p
                            key={index}
                            style={{
                                margin: '2px 0',
                                color: entry.color,
                            }}
                        >
                            {entry.name}: {formatNumber(entry.value)}
                            {entry.dataKey.includes('temperature') || entry.dataKey.includes('temp')
                                ? '°C'
                                : '%'}
                        </p>
                    ))}
                </div>
            );
        }
        return null;
    };

    // Custom dot component cho nhiệt độ - hiển thị màu khác nhau dựa trên status
    const CustomTemperatureDot = (props) => {
        const { cx, cy, payload } = props;
        const status = payload?.tempStatus || 'normal';

        // Chỉ hiển thị dot nếu không phải normal
        if (status === 'normal') {
            return null;
        }

        const color = status === 'danger' ? '#dc3545' : '#ffc107'; // Đỏ cho danger, vàng cho warning
        const radius = status === 'danger' ? 6 : 5; // Danger dot lớn hơn

        return (
            <circle
                cx={cx}
                cy={cy}
                r={radius}
                fill={color}
                stroke="#fff"
                strokeWidth={2}
            />
        );
    };

    if (!chartData || chartData.length === 0) {
        return (
            <div style={{ padding: '40px', textAlign: 'center', color: '#666' }}>
                <p>Chưa có dữ liệu để hiển thị</p>
                <p style={{ fontSize: '0.9rem', marginTop: '10px', color: '#999' }}>
                    Dữ liệu sẽ được tạo tự động mỗi 5 giây. Vui lòng đợi thêm dữ liệu.
                </p>
            </div>
        );
    }

    // Nếu chỉ có 1 điểm dữ liệu, vẫn hiển thị chart với 1 điểm (scatter-like)
    // Recharts có thể hiển thị 1 điểm, nhưng sẽ không có đường line

    return (
        <ResponsiveContainer width="100%" height={400}>
            <LineChart data={chartData} margin={{ top: 5, right: 30, left: 20, bottom: 5 }}>
                <CartesianGrid strokeDasharray="3 3" opacity={0.3} />

                {/* Vùng an toàn cho nhiệt độ */}
                <ReferenceArea
                    yAxisId="temp"
                    y1={safeRange.min}
                    y2={safeRange.max}
                    fill="#d4edda"
                    fillOpacity={0.2}
                    stroke="none"
                />

                {/* Reference lines cho ngưỡng an toàn */}
                <ReferenceLine
                    yAxisId="temp"
                    y={safeRange.min}
                    stroke="#28a745"
                    strokeDasharray="5 5"
                    label={{ value: 'Min', position: 'insideRight', fill: '#28a745' }}
                />
                <ReferenceLine
                    yAxisId="temp"
                    y={safeRange.max}
                    stroke="#28a745"
                    strokeDasharray="5 5"
                    label={{ value: 'Max', position: 'insideRight', fill: '#28a745' }}
                />

                <XAxis
                    dataKey="timeLabel"
                    tick={{ fontSize: 12 }}
                    interval="preserveStartEnd"
                />

                <YAxis
                    yAxisId="temp"
                    orientation="left"
                    label={{ value: 'Nhiệt độ (°C)', angle: -90, position: 'insideLeft' }}
                    domain={['dataMin - 2', 'dataMax + 2']}
                    tick={{ fontSize: 12 }}
                    tickFormatter={(value) => formatNumber(value)}
                />

                <YAxis
                    yAxisId="humidity"
                    orientation="right"
                    label={{ value: 'Độ ẩm (%)', angle: 90, position: 'insideRight' }}
                    domain={[0, 100]}
                    tick={{ fontSize: 12 }}
                    tickFormatter={(value) => formatNumber(value)}
                />

                <Tooltip content={<CustomTooltip />} />

                <Legend
                    content={(props) => {
                        const { payload } = props;
                        return (
                            <div style={{ textAlign: 'center', marginTop: '10px' }}>
                                <div style={{ display: 'flex', justifyContent: 'center', gap: '20px', flexWrap: 'wrap' }}>
                                    {payload?.map((entry, index) => (
                                        <span key={index} style={{ color: entry.color, fontSize: '12px' }}>
                                            <span style={{ marginRight: '5px' }}>{entry.value}</span>
                                        </span>
                                    ))}
                                    <span style={{ fontSize: '12px', marginLeft: '10px' }}>
                                        <span style={{ display: 'inline-block', width: '10px', height: '10px', borderRadius: '50%', backgroundColor: '#ffc107', marginRight: '5px' }}></span>
                                        Cảnh báo
                                    </span>
                                    <span style={{ fontSize: '12px' }}>
                                        <span style={{ display: 'inline-block', width: '10px', height: '10px', borderRadius: '50%', backgroundColor: '#dc3545', marginRight: '5px' }}></span>
                                        Nguy hiểm
                                    </span>
                                </div>
                            </div>
                        );
                    }}
                />

                {/* Nhiệt độ - Line chính */}
                <Line
                    yAxisId="temp"
                    type="monotone"
                    dataKey="temperature"
                    stroke="#ff6b6b"
                    strokeWidth={2}
                    dot={<CustomTemperatureDot />}
                    activeDot={{ r: 6 }}
                    name="Nhiệt độ"
                />

                {/* Độ ẩm */}
                <Line
                    yAxisId="humidity"
                    type="monotone"
                    dataKey="humidity"
                    stroke="#4ecdc4"
                    strokeWidth={2}
                    dot={false}
                    activeDot={{ r: 6 }}
                    name="Độ ẩm"
                />
            </LineChart>
        </ResponsiveContainer>
    );
};

export default DeviceChart;

