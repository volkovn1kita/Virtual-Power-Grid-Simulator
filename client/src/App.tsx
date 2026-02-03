import { useEffect, useState, useCallback } from 'react';
import { createTheme, ThemeProvider } from '@mui/material/styles';
import CssBaseline from '@mui/material/CssBaseline';
import { AppBar, Toolbar, Typography, Container, Paper, CircularProgress, Box, Slider, Chip, Fade, IconButton } from '@mui/material';
import Grid from '@mui/material/Grid'; 
import { XAxis, YAxis, CartesianGrid, Tooltip, ResponsiveContainer, ReferenceLine, Area, AreaChart } from 'recharts';
import PowerSettingsNewIcon from '@mui/icons-material/PowerSettingsNew';
import TrendingUpIcon from '@mui/icons-material/TrendingUp';
import TrendingDownIcon from '@mui/icons-material/TrendingDown';
import FactoryIcon from '@mui/icons-material/Factory';
import WhatshotIcon from '@mui/icons-material/Whatshot';
import WaterIcon from '@mui/icons-material/Water';
import WbSunnyIcon from '@mui/icons-material/WbSunny';
import AirIcon from '@mui/icons-material/Air';
import FlashOnIcon from '@mui/icons-material/FlashOn';
import HomeWorkIcon from '@mui/icons-material/HomeWork';
import BarChartIcon from '@mui/icons-material/BarChart';
import CloudIcon from '@mui/icons-material/Cloud';
import LightbulbIcon from '@mui/icons-material/Lightbulb';
import LightbulbOutlinedIcon from '@mui/icons-material/LightbulbOutlined';

import type { GridSnapshot, PowerPlant, PowerConsumer } from './types/models';
import { powerGridApi } from './services/api';

const darkTheme = createTheme({
  palette: {
    mode: 'dark',
    primary: { main: '#60a5fa' },
    background: { 
      default: 'linear-gradient(135deg, #0f172a 0%, #1e293b 100%)', 
      paper: 'rgba(30, 41, 59, 0.7)' 
    },
    error: { main: '#f87171' },
    success: { main: '#34d399' },
    warning: { main: '#fbbf24' },
  },
  typography: {
    fontFamily: '"Inter", "Roboto", "Helvetica", "Arial", sans-serif',
  },
  components: {
    MuiPaper: {
      styleOverrides: {
        root: {
          backgroundImage: 'none',
          backdropFilter: 'blur(20px)',
          border: '1px solid rgba(148, 163, 184, 0.1)',
        },
      },
    },
  },
});

const isWeatherDependent = (type: number) => {
    return type === 3 || type === 4;
};

const getPlantIcon = (type: number) => {
    const iconProps = { sx: { fontSize: '24px' } };
    switch(type) {
        case 0: return <FactoryIcon {...iconProps} />; // Thermal
        case 1: return <WhatshotIcon  {...iconProps} />; // Nuclear
        case 2: return <WaterIcon {...iconProps} />; // Hydro
        case 3: return <WbSunnyIcon {...iconProps} />; // Solar
        case 4: return <AirIcon {...iconProps} />; // Wind
        default: return <FlashOnIcon {...iconProps} />;
    }
};

const getPlantGradient = (type: number, isWorking: boolean) => {
    if (!isWorking) return 'linear-gradient(135deg, rgba(71, 85, 105, 0.3) 0%, rgba(51, 65, 85, 0.3) 100%)';
    
    switch(type) {
        case 0: return 'linear-gradient(135deg, rgba(239, 68, 68, 0.2) 0%, rgba(220, 38, 38, 0.3) 100%)';
        case 1: return 'linear-gradient(135deg, rgba(168, 85, 247, 0.2) 0%, rgba(147, 51, 234, 0.3) 100%)';
        case 2: return 'linear-gradient(135deg, rgba(59, 130, 246, 0.2) 0%, rgba(37, 99, 235, 0.3) 100%)';
        case 3: return 'linear-gradient(135deg, rgba(251, 191, 36, 0.2) 0%, rgba(245, 158, 11, 0.3) 100%)';
        case 4: return 'linear-gradient(135deg, rgba(34, 211, 238, 0.2) 0%, rgba(6, 182, 212, 0.3) 100%)';
        default: return 'linear-gradient(135deg, rgba(71, 85, 105, 0.2) 0%, rgba(51, 65, 85, 0.3) 100%)';
    }
};

function App() {
  const [snapshot, setSnapshot] = useState<GridSnapshot | null>(null);
  const [plants, setPlants] = useState<PowerPlant[]>([]);
  const [consumers, setConsumers] = useState<PowerConsumer[]>([]);
  const [history, setHistory] = useState<{ time: string; freq: number }[]>([]);
  const [loading, setLoading] = useState<boolean>(true);
  //const [simTime, setSimTime] = useState<number>(0);

  const fetchData = useCallback(async () => {
    try {
      const [statusData, plantsData, consumersData] = await Promise.all([
          powerGridApi.getGridStatus(),
          powerGridApi.getPlants(),
          powerGridApi.getConsumers()
      ]);

      setSnapshot(statusData);
      setPlants(plantsData);
      setConsumers(consumersData);

      setHistory(prev => {
      const date = new Date(statusData.timeStamp);

      // формат HH:mm:ss або HH:mm
      const timeLabel = date.toLocaleTimeString('en-GB', {
        hour12: false,
        hour: '2-digit',
        minute: '2-digit',
      });

      const newPoint = {
        time: timeLabel,
        freq: statusData.frequency,
      };

      const newHistory = [...prev, newPoint];
      if (newHistory.length > 60) newHistory.shift();

      return newHistory;
    });


    } catch (error) {
      console.error("Помилка API:", error);
    } finally {
      setLoading(false);
    }
  }, []);

  useEffect(() => {
    fetchData();
    const interval = setInterval(fetchData, 5000); 
    return () => clearInterval(interval);
  }, [fetchData]);

  const handlePlantToggle = async (id: string, isWorking: boolean) => {
    try {
        if (isWorking) await powerGridApi.turnOffPlant(id);
        else await powerGridApi.turnOnPlant(id);
        fetchData();
    } catch (error) {
        console.error("Error toggling plant:", error);
    }
  };

  const handlePowerChange = async (id: string, newValue: number | number[]) => {
      try {
          const power = newValue as number;
          await powerGridApi.adjustPower(id, power);
          fetchData();
      } catch (error) {
          console.error("Error adjusting power:", error);
      }
  };

  const getFreqColor = (freq: number) => {
    if (freq >= 49.9 && freq <= 50.1) return '#34d399'; 
    if (freq >= 49.5 && freq <= 50.5) return '#fbbf24'; 
    return '#f87171'; 
  };

  const getFreqStatus = (freq: number) => {
    if (freq >= 49.9 && freq <= 50.1) return 'OPTIMAL';
    if (freq >= 49.5 && freq <= 50.5) return 'STABLE';
    return 'CRITICAL';
  };

  const balance = (snapshot?.totalGeneration || 0) - (snapshot?.totalDemand || 0);

  return (
    <ThemeProvider theme={darkTheme}>
      <CssBaseline />
      
      <Box sx={{ 
        minHeight: '100vh',
        background: 'linear-gradient(135deg, #0f172a 0%, #1e293b 50%, #0f172a 100%)',
        position: 'relative',
        '&::before': {
          content: '""',
          position: 'absolute',
          top: 0,
          left: 0,
          right: 0,
          bottom: 0,
          background: 'radial-gradient(circle at 20% 50%, rgba(96, 165, 250, 0.1) 0%, transparent 50%), radial-gradient(circle at 80% 80%, rgba(168, 85, 247, 0.1) 0%, transparent 50%)',
          pointerEvents: 'none',
        }
      }}>
        <AppBar position="static" elevation={0} sx={{ 
          background: 'rgba(15, 23, 42, 0.8)',
          backdropFilter: 'blur(20px)',
          borderBottom: '1px solid rgba(148, 163, 184, 0.1)'
        }}>
          <Toolbar sx={{ py: 1 }}>
            <Box sx={{ display: 'flex', alignItems: 'center', gap: 2, flexGrow: 1 }}>
              <Box sx={{ 
                width: 48, 
                height: 48, 
                borderRadius: 2,
                background: 'linear-gradient(135deg, #60a5fa 0%, #3b82f6 100%)',
                display: 'flex',
                alignItems: 'center',
                justifyContent: 'center',
                boxShadow: '0 4px 20px rgba(96, 165, 250, 0.4)'
              }}>
                <FlashOnIcon sx={{ fontSize: '28px', color: 'white' }} />
              </Box>
              <Box>
                <Typography variant="h5" component="div" sx={{ fontWeight: 700, letterSpacing: '-0.5px' }}>
                  Power Grid Control
                </Typography>
                <Typography variant="caption" sx={{ color: 'rgba(148, 163, 184, 0.8)', fontSize: '12px' }}>
                  Energy Management System
                </Typography>
              </Box>
            </Box>
            
            <Box sx={{ 
              display: 'flex', 
              alignItems: 'center', 
              gap: 3,
              px: 3,
              py: 1.5,
              borderRadius: 3,
              background: 'rgba(30, 41, 59, 0.6)',
              border: `2px solid ${snapshot ? getFreqColor(snapshot.frequency) : 'rgba(148, 163, 184, 0.2)'}`,
              boxShadow: `0 0 30px ${snapshot ? getFreqColor(snapshot.frequency) : 'transparent'}40`
            }}>
              <Box sx={{ textAlign: 'center' }}>
                <Typography variant="caption" sx={{ color: 'rgba(148, 163, 184, 0.8)', fontSize: '10px', textTransform: 'uppercase', letterSpacing: '1px', fontWeight: 600 }}>
                  Grid Frequency
                </Typography>
                <Box sx={{ display: 'flex', alignItems: 'baseline', gap: 0.5, mt: 0.5 }}>
                  <Typography variant="h3" sx={{ 
                    color: snapshot ? getFreqColor(snapshot.frequency) : 'gray', 
                    fontWeight: 800, 
                    fontFamily: '"JetBrains Mono", monospace',
                    lineHeight: 1,
                    textShadow: `0 0 20px ${snapshot ? getFreqColor(snapshot.frequency) : 'transparent'}80`
                  }}>
                    {snapshot ? `${snapshot.frequency.toFixed(2)}` : '--'}
                  </Typography>
                  <Typography variant="body2" sx={{ color: 'rgba(148, 163, 184, 0.6)', fontWeight: 600 }}>Hz</Typography>
                </Box>
                <Chip 
                  label={snapshot ? getFreqStatus(snapshot.frequency) : 'N/A'} 
                  size="small" 
                  sx={{ 
                    mt: 0.5,
                    height: 20,
                    fontSize: '9px',
                    fontWeight: 700,
                    background: snapshot ? getFreqColor(snapshot.frequency) : 'gray',
                    color: '#0f172a',
                    letterSpacing: '0.5px'
                  }} 
                />
              </Box>
            </Box>
          </Toolbar>
        </AppBar>

        <Container maxWidth="xl" sx={{ mt: 3, mb: 3, position: 'relative', zIndex: 1 }}>
          {loading ? (
            <Box display="flex" justifyContent="center" alignItems="center" height="80vh">
              <CircularProgress size={60} sx={{ color: '#60a5fa' }} />
            </Box>
          ) : (
            <Fade in={!loading} timeout={800}>
              <Grid container spacing={3}>
                
                {/* GENERATORS */}
                <Grid item xs={12} lg={3}>
                  <Paper sx={{ 
                    p: 2.5, 
                    height: 'calc(100vh - 180px)', 
                    overflowY: 'auto',
                    background: 'rgba(30, 41, 59, 0.4)',
                    '&::-webkit-scrollbar': { width: '6px' },
                    '&::-webkit-scrollbar-track': { background: 'rgba(71, 85, 105, 0.2)', borderRadius: '10px' },
                    '&::-webkit-scrollbar-thumb': { background: 'rgba(96, 165, 250, 0.5)', borderRadius: '10px' },
                  }}>
                    <Box sx={{ display: 'flex', alignItems: 'center', gap: 1.5, mb: 2.5 }}>
                      <Box sx={{ 
                        width: 36, 
                        height: 36, 
                        borderRadius: 2,
                        background: 'linear-gradient(135deg, #34d399 0%, #10b981 100%)',
                        display: 'flex',
                        alignItems: 'center',
                        justifyContent: 'center',
                      }}>
                        <FactoryIcon sx={{ fontSize: '20px', color: 'white' }} />
                      </Box>
                      <Typography variant="h6" sx={{ fontWeight: 700, letterSpacing: '-0.3px' }}>
                        Power Plants
                      </Typography>
                    </Box>
                    
                    {[...plants].sort((a, b) => a.id.localeCompare(b.id)).map((plant, idx) => (
                      <Fade in={true} timeout={300 + idx * 100} key={plant.id}>
                        <Box sx={{ 
                          mb: 2, 
                          p: 2, 
                          borderRadius: 3,
                          background: getPlantGradient(plant.type, plant.isWorking),
                          border: plant.isWorking ? '1px solid rgba(148, 163, 184, 0.2)' : '1px solid rgba(71, 85, 105, 0.3)',
                          transition: 'all 0.3s cubic-bezier(0.4, 0, 0.2, 1)',
                          position: 'relative',
                          overflow: 'hidden',
                          '&:hover': {
                            transform: 'translateY(-2px)',
                            boxShadow: '0 8px 30px rgba(0, 0, 0, 0.3)',
                          },
                          '&::before': {
                            content: '""',
                            position: 'absolute',
                            top: 0,
                            left: 0,
                            right: 0,
                            height: '3px',
                            background: plant.isWorking ? 'linear-gradient(90deg, #34d399, #60a5fa)' : 'rgba(71, 85, 105, 0.5)',
                          }
                        }}>
                          
                          <Box display="flex" justifyContent="space-between" alignItems="center" mb={1.5}>
                            <Box display="flex" alignItems="center" gap={1.5}>
                              <Box sx={{ color: 'white', display: 'flex', alignItems: 'center' }}>
                                {getPlantIcon(plant.type)}
                              </Box>
                              <Box>
                                <Typography variant="subtitle2" fontWeight={700} sx={{ fontSize: '14px', letterSpacing: '-0.2px' }}>
                                  {plant.name}
                                </Typography>
                                <Typography variant="caption" sx={{ color: 'rgba(148, 163, 184, 0.7)', fontSize: '11px' }}>
                                  Capacity: {plant.maxCapacity} MW
                                </Typography>
                              </Box>
                            </Box>
                            <IconButton 
                              onClick={() => handlePlantToggle(plant.id, plant.isWorking)}
                              size="small"
                              sx={{
                                background: plant.isWorking ? 'rgba(52, 211, 153, 0.2)' : 'rgba(71, 85, 105, 0.3)',
                                border: plant.isWorking ? '2px solid #34d399' : '2px solid rgba(148, 163, 184, 0.3)',
                                '&:hover': {
                                  background: plant.isWorking ? 'rgba(52, 211, 153, 0.3)' : 'rgba(71, 85, 105, 0.5)',
                                }
                              }}
                            >
                              <PowerSettingsNewIcon sx={{ color: plant.isWorking ? '#34d399' : 'rgba(148, 163, 184, 0.6)', fontSize: '20px' }} />
                            </IconButton>
                          </Box>

                          <Box sx={{ 
                            display: 'flex', 
                            alignItems: 'center', 
                            gap: 2,
                            p: 1.5,
                            borderRadius: 2,
                            background: 'rgba(15, 23, 42, 0.4)',
                            mb: 1.5
                          }}>
                            <Box sx={{ flex: 1 }}>
                              <Typography variant="caption" sx={{ color: 'rgba(148, 163, 184, 0.7)', fontSize: '10px', textTransform: 'uppercase', letterSpacing: '0.5px' }}>
                                Output
                              </Typography>
                              <Typography variant="h6" fontWeight={700} sx={{ color: '#60a5fa', fontFamily: '"JetBrains Mono", monospace' }}>
                                {plant.currentPower.toFixed(1)}
                                <span style={{ fontSize: '12px', color: 'rgba(148, 163, 184, 0.6)', marginLeft: '4px' }}>MW</span>
                              </Typography>
                            </Box>
                            <Box sx={{ 
                              px: 1.5, 
                              py: 0.5, 
                              borderRadius: 2,
                              background: plant.isWorking ? 'rgba(52, 211, 153, 0.2)' : 'rgba(71, 85, 105, 0.3)',
                              border: plant.isWorking ? '1px solid rgba(52, 211, 153, 0.5)' : '1px solid rgba(148, 163, 184, 0.2)'
                            }}>
                              <Typography variant="caption" fontWeight={700} sx={{ 
                                color: plant.isWorking ? '#34d399' : 'rgba(148, 163, 184, 0.6)',
                                fontSize: '11px'
                              }}>
                                {((plant.currentPower / plant.maxCapacity) * 100).toFixed(0)}%
                              </Typography>
                            </Box>
                          </Box>
                          
                          {isWeatherDependent(plant.type) ? (
                            <Box sx={{
                              p: 1.5,
                              borderRadius: 2,
                              background: 'rgba(251, 191, 36, 0.1)',
                              border: '1px dashed rgba(251, 191, 36, 0.3)',
                              display: 'flex',
                              alignItems: 'center',
                              gap: 1
                            }}>
                              <CloudIcon sx={{ fontSize: '18px', color: '#fbbf24' }} />
                              <Typography variant="caption" sx={{ color: '#fbbf24', fontWeight: 600, fontSize: '11px' }}>
                                Weather Dependent
                              </Typography>
                            </Box>
                          ) : (
                            <Box sx={{ px: 0.5 }}>
                              <Slider
                                disabled={!plant.isWorking}
                                defaultValue={plant.currentPower}
                                key={`slider-${plant.id}-${plant.currentPower}`}
                                step={1}
                                min={0}
                                max={plant.maxCapacity}
                                onChangeCommitted={(_, val) => handlePowerChange(plant.id, val)}
                                sx={{
                                  color: plant.isWorking ? '#60a5fa' : 'rgba(71, 85, 105, 0.5)',
                                  height: 6,
                                  '& .MuiSlider-thumb': {
                                    width: 16,
                                    height: 16,
                                    background: plant.isWorking ? 'linear-gradient(135deg, #60a5fa, #3b82f6)' : 'rgba(71, 85, 105, 0.5)',
                                    border: '2px solid rgba(15, 23, 42, 0.8)',
                                    boxShadow: plant.isWorking ? '0 0 10px rgba(96, 165, 250, 0.5)' : 'none',
                                    '&:hover': {
                                      boxShadow: plant.isWorking ? '0 0 15px rgba(96, 165, 250, 0.8)' : 'none',
                                    }
                                  },
                                  '& .MuiSlider-track': {
                                    background: plant.isWorking ? 'linear-gradient(90deg, #60a5fa, #3b82f6)' : 'rgba(71, 85, 105, 0.3)',
                                    border: 'none',
                                  },
                                  '& .MuiSlider-rail': {
                                    background: 'rgba(71, 85, 105, 0.3)',
                                  }
                                }}
                              />
                            </Box>
                          )}

                          <Box sx={{ 
                            width: '100%', 
                            height: 6, 
                            bgcolor: 'rgba(71, 85, 105, 0.3)', 
                            mt: 1.5, 
                            borderRadius: 3,
                            overflow: 'hidden',
                            position: 'relative'
                          }}>
                            <Box sx={{
                              position: 'absolute',
                              top: 0,
                              left: 0,
                              width: `${(plant.currentPower / plant.maxCapacity) * 100}%`,
                              height: '100%',
                              background: plant.isWorking 
                                ? (isWeatherDependent(plant.type) 
                                  ? 'linear-gradient(90deg, #fbbf24, #f59e0b)' 
                                  : 'linear-gradient(90deg, #34d399, #10b981)')
                                : 'transparent',
                              borderRadius: 3,
                              transition: 'width 0.5s cubic-bezier(0.4, 0, 0.2, 1)',
                              boxShadow: plant.isWorking ? '0 0 10px currentColor' : 'none',
                            }} />
                          </Box>
                        </Box>
                      </Fade>
                    ))}
                  </Paper>
                </Grid>

                {/* BALANCE & CHART */}
                <Grid item xs={12} lg={6}>
                  <Grid container spacing={3} sx={{ height: 'calc(100vh - 180px)' }}>
                    
                    {/* Balance Cards */}
                    <Grid item xs={12}>
                      <Grid container spacing={2}>
                        <Grid item xs={12} md={4}>
                          <Paper sx={{ 
                            p: 2.5, 
                            background: 'rgba(52, 211, 153, 0.1)',
                            border: '1px solid rgba(52, 211, 153, 0.3)',
                            position: 'relative',
                            overflow: 'hidden',
                            '&::before': {
                              content: '""',
                              position: 'absolute',
                              top: 0,
                              left: 0,
                              right: 0,
                              height: '3px',
                              background: 'linear-gradient(90deg, #34d399, #10b981)',
                            }
                          }}>
                            <Box display="flex" alignItems="center" gap={1} mb={1}>
                              <TrendingUpIcon sx={{ color: '#34d399', fontSize: '20px' }} />
                              <Typography variant="caption" sx={{ 
                                color: 'rgba(148, 163, 184, 0.8)', 
                                fontSize: '11px',
                                textTransform: 'uppercase',
                                letterSpacing: '1px',
                                fontWeight: 600
                              }}>
                                Generation
                              </Typography>
                            </Box>
                            <Typography variant="h4" fontWeight={800} sx={{ 
                              color: '#34d399',
                              fontFamily: '"JetBrains Mono", monospace',
                              letterSpacing: '-1px'
                            }}>
                              {snapshot?.totalGeneration.toFixed(1)}
                              <span style={{ fontSize: '16px', color: 'rgba(148, 163, 184, 0.6)', marginLeft: '6px' }}>MW</span>
                            </Typography>
                          </Paper>
                        </Grid>

                        <Grid item xs={12} md={4}>
                          <Paper sx={{ 
                            p: 2.5,
                            background: balance >= 0 ? 'rgba(52, 211, 153, 0.15)' : 'rgba(248, 113, 113, 0.15)',
                            border: balance >= 0 ? '2px solid rgba(52, 211, 153, 0.4)' : '2px solid rgba(248, 113, 113, 0.4)',
                            position: 'relative',
                            overflow: 'hidden',
                            '&::before': {
                              content: '""',
                              position: 'absolute',
                              top: 0,
                              left: 0,
                              right: 0,
                              height: '3px',
                              background: balance >= 0 
                                ? 'linear-gradient(90deg, #34d399, #10b981)' 
                                : 'linear-gradient(90deg, #f87171, #ef4444)',
                            }
                          }}>
                            <Box display="flex" alignItems="center" gap={1} mb={1}>
                              <Box sx={{
                                width: 20,
                                height: 20,
                                borderRadius: '50%',
                                background: balance >= 0 ? '#34d399' : '#f87171',
                                display: 'flex',
                                alignItems: 'center',
                                justifyContent: 'center',
                                fontSize: '12px',
                                animation: 'pulse 2s infinite'
                              }}>
                                {balance >= 0 ? '✓' : '!'}
                              </Box>
                              <Typography variant="caption" sx={{ 
                                color: 'rgba(148, 163, 184, 0.8)', 
                                fontSize: '11px',
                                textTransform: 'uppercase',
                                letterSpacing: '1px',
                                fontWeight: 600
                              }}>
                                Balance
                              </Typography>
                            </Box>
                            <Typography variant="h4" fontWeight={800} sx={{ 
                              color: balance >= 0 ? '#34d399' : '#f87171',
                              fontFamily: '"JetBrains Mono", monospace',
                              letterSpacing: '-1px'
                            }}>
                              {balance > 0 ? '+' : ''}{balance.toFixed(2)}
                              <span style={{ fontSize: '16px', color: 'rgba(148, 163, 184, 0.6)', marginLeft: '6px' }}>MW</span>
                            </Typography>
                          </Paper>
                        </Grid>

                        <Grid item xs={12} md={4}>
                          <Paper sx={{ 
                            p: 2.5,
                            background: 'rgba(248, 113, 113, 0.1)',
                            border: '1px solid rgba(248, 113, 113, 0.3)',
                            position: 'relative',
                            overflow: 'hidden',
                            '&::before': {
                              content: '""',
                              position: 'absolute',
                              top: 0,
                              left: 0,
                              right: 0,
                              height: '3px',
                              background: 'linear-gradient(90deg, #f87171, #ef4444)',
                            }
                          }}>
                            <Box display="flex" alignItems="center" gap={1} mb={1}>
                              <TrendingDownIcon sx={{ color: '#f87171', fontSize: '20px' }} />
                              <Typography variant="caption" sx={{ 
                                color: 'rgba(148, 163, 184, 0.8)', 
                                fontSize: '11px',
                                textTransform: 'uppercase',
                                letterSpacing: '1px',
                                fontWeight: 600
                              }}>
                                Demand
                              </Typography>
                            </Box>
                            <Typography variant="h4" fontWeight={800} sx={{ 
                              color: '#f87171',
                              fontFamily: '"JetBrains Mono", monospace',
                              letterSpacing: '-1px'
                            }}>
                              {snapshot?.totalDemand.toFixed(1)}
                              <span style={{ fontSize: '16px', color: 'rgba(148, 163, 184, 0.6)', marginLeft: '6px' }}>MW</span>
                            </Typography>
                          </Paper>
                        </Grid>
                      </Grid>
                    </Grid>

                    {/* Chart */}
                    <Grid item xs={12} sx={{ flexGrow: 1 }}>
                      <Paper sx={{ 
                        p: 3, 
                        height: '100%',
                        background: 'rgba(30, 41, 59, 0.4)',
                        display: 'flex',
                        flexDirection: 'column'
                      }}>
                        <Box display="flex" alignItems="center" gap={2} mb={2}>
                          <Box sx={{ 
                            width: 36, 
                            height: 36, 
                            borderRadius: 2,
                            background: 'linear-gradient(135deg, #f59e0b 0%, #d97706 100%)',
                            display: 'flex',
                            alignItems: 'center',
                            justifyContent: 'center',
                          }}>
                            <BarChartIcon sx={{ fontSize: '20px', color: 'white' }} />
                          </Box>
                          <Box>
                            <Typography variant="h6" sx={{ fontWeight: 700, letterSpacing: '-0.3px' }}>
                              Frequency Monitor
                            </Typography>
                            <Typography variant="caption" sx={{ color: 'rgba(148, 163, 184, 0.7)', fontSize: '11px' }}>
                              Real-time grid stability analysis
                            </Typography>
                          </Box>
                        </Box>
                        
                        <Box sx={{ flexGrow: 1, minHeight: 0 }}>
                          <ResponsiveContainer width="100%" height="100%">
                            <AreaChart data={history}>
                              <defs>
                                <linearGradient id="colorFreq" x1="0" y1="0" x2="0" y2="1">
                                  <stop offset="5%" stopColor="#f59e0b" stopOpacity={0.3}/>
                                  <stop offset="95%" stopColor="#f59e0b" stopOpacity={0}/>
                                </linearGradient>
                              </defs>
                              <CartesianGrid strokeDasharray="3 3" stroke="rgba(148, 163, 184, 0.1)" />
                              <XAxis 
                                dataKey="time" 
                                stroke="rgba(148, 163, 184, 0.5)" 
                                fontSize={11}
                                tick={{fill: 'rgba(148, 163, 184, 0.7)'}}
                              />
                              <YAxis 
                                domain={[49, 51]} 
                                stroke="rgba(148, 163, 184, 0.5)" 
                                fontSize={11}
                                tick={{fill: 'rgba(148, 163, 184, 0.7)'}}
                              />
                              <Tooltip 
                                contentStyle={{ 
                                  backgroundColor: 'rgba(15, 23, 42, 0.95)', 
                                  border: '1px solid rgba(148, 163, 184, 0.2)',
                                  borderRadius: '8px',
                                  backdropFilter: 'blur(10px)'
                                }} 
                                itemStyle={{ color: '#f59e0b', fontWeight: 600 }}
                                labelStyle={{ color: 'rgba(148, 163, 184, 0.8)' }}
                              />
                              <ReferenceLine 
                                y={50} 
                                stroke="#34d399" 
                                strokeDasharray="5 5" 
                                strokeWidth={2}
                              />
                              <Area 
                                type="monotone" 
                                dataKey="freq" 
                                stroke="#f59e0b" 
                                strokeWidth={3}
                                fill="url(#colorFreq)"
                                isAnimationActive={false}
                              />
                            </AreaChart>
                          </ResponsiveContainer>
                        </Box>
                      </Paper>
                    </Grid>
                  </Grid>
                </Grid>

                {/* CONSUMERS */}
                <Grid item xs={12} lg={3}>
                  <Paper sx={{ 
                    p: 2.5, 
                    height: 'calc(100vh - 180px)', 
                    overflowY: 'auto',
                    background: 'rgba(30, 41, 59, 0.4)',
                    '&::-webkit-scrollbar': { width: '6px' },
                    '&::-webkit-scrollbar-track': { background: 'rgba(71, 85, 105, 0.2)', borderRadius: '10px' },
                    '&::-webkit-scrollbar-thumb': { background: 'rgba(248, 113, 113, 0.5)', borderRadius: '10px' },
                  }}>
                    <Box sx={{ display: 'flex', alignItems: 'center', gap: 1.5, mb: 2.5 }}>
                      <Box sx={{ 
                        width: 36, 
                        height: 36, 
                        borderRadius: 2,
                        background: 'linear-gradient(135deg, #f87171 0%, #ef4444 100%)',
                        display: 'flex',
                        alignItems: 'center',
                        justifyContent: 'center',
                      }}>
                        <HomeWorkIcon sx={{ fontSize: '20px', color: 'white' }} />
                      </Box>
                      <Typography variant="h6" sx={{ fontWeight: 700, letterSpacing: '-0.3px' }}>
                        Load Distribution
                      </Typography>
                    </Box>

                    {[...consumers].sort((a, b) => a.id.localeCompare(b.id)).map((consumer, idx) => (
                      <Fade in={true} timeout={300 + idx * 100} key={consumer.id}>
                        <Box sx={{
                          mb: 2,
                          p: 2,
                          borderRadius: 3,
                          background: consumer.isActive 
                            ? 'linear-gradient(135deg, rgba(59, 130, 246, 0.1) 0%, rgba(37, 99, 235, 0.15) 100%)'
                            : 'linear-gradient(135deg, rgba(248, 113, 113, 0.1) 0%, rgba(220, 38, 38, 0.15) 100%)',
                          border: consumer.isActive 
                            ? '1px solid rgba(59, 130, 246, 0.3)' 
                            : '1px solid rgba(248, 113, 113, 0.3)',
                          transition: 'all 0.3s cubic-bezier(0.4, 0, 0.2, 1)',
                          position: 'relative',
                          overflow: 'hidden',
                          opacity: consumer.isActive ? 1 : 0.7,
                          '&:hover': {
                            transform: 'translateX(4px)',
                            boxShadow: consumer.isActive 
                              ? '0 4px 20px rgba(59, 130, 246, 0.2)' 
                              : '0 4px 20px rgba(248, 113, 113, 0.2)',
                          },
                          '&::before': {
                            content: '""',
                            position: 'absolute',
                            top: 0,
                            left: 0,
                            right: 0,
                            height: '3px',
                            background: consumer.isActive 
                              ? 'linear-gradient(90deg, #60a5fa, #3b82f6)' 
                              : 'linear-gradient(90deg, #f87171, #ef4444)',
                          }
                        }}>
                          <Box display="flex" justifyContent="space-between" alignItems="flex-start" mb={1.5}>
                            <Box>
                              <Typography variant="subtitle2" fontWeight={700} sx={{ 
                                fontSize: '14px',
                                letterSpacing: '-0.2px',
                                color: consumer.isActive ? 'white' : 'rgba(248, 113, 113, 0.9)'
                              }}>
                                {consumer.name}
                              </Typography>
                              <Typography variant="caption" sx={{ 
                                color: 'rgba(148, 163, 184, 0.7)',
                                fontSize: '11px'
                              }}>
                                Load: <span style={{ color: consumer.isActive ? '#60a5fa' : '#f87171', fontWeight: 700 }}>
                                  {consumer.maxPeakLoad.toFixed(1)} MW
                                </span>
                              </Typography>
                            </Box>
                            <Box sx={{ 
                              width: 28,
                              height: 28,
                              borderRadius: '50%',
                              background: consumer.isActive 
                                ? 'linear-gradient(135deg, #fbbf24, #f59e0b)'
                                : 'rgba(71, 85, 105, 0.3)',
                              display: 'flex',
                              alignItems: 'center',
                              justifyContent: 'center',
                              fontSize: '11px',
                              fontWeight: 700,
                              color: consumer.isActive ? '#0f172a' : 'rgba(148, 163, 184, 0.6)',
                              border: consumer.isActive ? '2px solid rgba(251, 191, 36, 0.5)' : '1px solid rgba(148, 163, 184, 0.3)'
                            }}>
                              P{consumer.priority}
                            </Box>
                          </Box>

                          <Box sx={{
                            display: 'flex',
                            alignItems: 'center',
                            gap: 1.5,
                            p: 1.5,
                            borderRadius: 2,
                            background: 'rgba(15, 23, 42, 0.4)',
                          }}>
                            <Box sx={{
                              display: 'flex',
                              alignItems: 'center',
                              color: consumer.isActive ? '#34d399' : '#f87171'
                            }}>
                              {consumer.isActive ? (
                                <LightbulbIcon sx={{ fontSize: '20px' }} />
                              ) : (
                                <LightbulbOutlinedIcon sx={{ fontSize: '20px' }} />
                              )}
                            </Box>
                            <Typography variant="caption" fontWeight={700} sx={{
                              color: consumer.isActive ? '#34d399' : '#f87171',
                              fontSize: '12px',
                              textTransform: 'uppercase',
                              letterSpacing: '0.5px'
                            }}>
                              {consumer.isActive ? 'Online' : 'Offline'}
                            </Typography>
                          </Box>
                        </Box>
                      </Fade>
                    ))}
                  </Paper>
                </Grid>
              </Grid>
            </Fade>
          )}
        </Container>

        <style>
          {`
            @import url('https://fonts.googleapis.com/css2?family=Inter:wght@400;600;700;800&family=JetBrains+Mono:wght@700&display=swap');
            
            @keyframes pulse {
              0%, 100% {
                opacity: 1;
              }
              50% {
                opacity: 0.7;
              }
            }
          `}
        </style>
      </Box>
    </ThemeProvider>
  );
}

export default App;