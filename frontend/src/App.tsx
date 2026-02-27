import { useEffect, useState, useCallback } from 'react';
import { useDropzone } from 'react-dropzone';
import { apiClient } from './api';
import { 
  Table, TableBody, TableCell, TableHead, TableRow, 
  Button, TextField, Paper, Typography, Box 
} from '@mui/material';

export default function App() {
  const [orders, setOrders] = useState<any[]>([]);
  const [formData, setFormData] = useState({ lat: '', lon: '', subtotal: '' });

  const fetchOrders = useCallback(() => {
    apiClient.get('/orders?page=1&pageSize=50')
      .then(res => setOrders(res.data.items || []))
      .catch(err => console.error("Помилка завантаження:", err));
  }, []);

  useEffect(() => {
    fetchOrders();
  }, [fetchOrders]);

  const handleManualCreate = async (e: any) => {
    e.preventDefault();
    try {
      await apiClient.post('/orders', {
        latitude: parseFloat(formData.lat),
        longitude: parseFloat(formData.lon),
        subtotal: parseFloat(formData.subtotal),
        timestamp: new Date().toISOString()
      });
      fetchOrders();
      setFormData({ lat: '', lon: '', subtotal: '' });
    } catch (error) {
      alert("Не вдалося створити замовлення.");
    }
  };

  const onDrop = useCallback(async (acceptedFiles: any[]) => {
    const file = acceptedFiles[0];
    if (!file) return;
    const data = new FormData();
    data.append('file', file);
    try {
      await apiClient.post('/orders/import', data);
      fetchOrders();
      alert('Файл успішно завантажено!');
    } catch (error) {
      alert('Помилка завантаження файлу.');
    }
  }, [fetchOrders]);

  const { getRootProps, getInputProps } = useDropzone({ onDrop });

  return (
    <Box sx={{ p: 4, backgroundColor: '#f5f5f5', minHeight: '100vh' }}>
      <Typography variant="h4" gutterBottom sx={{ fontWeight: 'bold', mb: 4 }}>
        BetterMe: Адмін-панель податків
      </Typography>

      {/* Заміна Grid на Flexbox */}
      <Box sx={{ display: 'flex', gap: 3, mb: 4, flexWrap: 'wrap' }}>
        
        {/* Форма створення */}
        <Paper sx={{ p: 3, flex: 1, minWidth: '350px' }}>
          <Typography variant="h6" gutterBottom>Створити замовлення вручну</Typography>
          <form onSubmit={handleManualCreate} style={{ display: 'flex', flexDirection: 'column', gap: '15px' }}>
            <TextField label="Latitude" value={formData.lat} onChange={(e: any) => setFormData({...formData, lat: e.target.value})} required />
            <TextField label="Longitude" value={formData.lon} onChange={(e: any) => setFormData({...formData, lon: e.target.value})} required />
            <TextField label="Subtotal" type="number" value={formData.subtotal} onChange={(e: any) => setFormData({...formData, subtotal: e.target.value})} required />
            <Button type="submit" variant="contained">РОЗРАХУВАТИ ТА ЗБЕРЕГТИ</Button>
          </form>
        </Paper>

        {/* Завантаження CSV */}
        <Paper {...getRootProps()} sx={{ 
          p: 3, border: '2px dashed #1976d2', flex: 1, minWidth: '300px',
          display: 'flex', alignItems: 'center', justifyContent: 'center', cursor: 'pointer' 
        }}>
          <input {...getInputProps()} />
          <Typography align="center" color="primary">Перетягни сюди CSV файл або клікніть для вибору</Typography>
        </Paper>
      </Box>

      {/* Таблиця */}
      <Paper sx={{ p: 2 }}>
        <Typography variant="h6" gutterBottom>Список замовлень</Typography>
        <Table size="small">
          <TableHead sx={{ backgroundColor: '#f5f5f5' }}>
            <TableRow>
              <TableCell><b>ID</b></TableCell>
              <TableCell><b>Ціна</b></TableCell>
              <TableCell><b>Ставка</b></TableCell>
              <TableCell><b>Податок</b></TableCell>
              <TableCell><b>Разом</b></TableCell>
              <TableCell><b>Юрисдикції</b></TableCell>
            </TableRow>
          </TableHead>
          <TableBody>
            {orders.map((o: any) => (
              <TableRow key={o.id}>
                <TableCell sx={{ fontSize: '0.75rem' }}>{o.id?.substring(0, 8)}...</TableCell>
                <TableCell>${o.subtotal?.toFixed(2)}</TableCell>
                <TableCell>{(o.compositeTaxRate * 100).toFixed(3)}%</TableCell>
                <TableCell sx={{ color: 'green' }}>${o.taxAmount?.toFixed(2)}</TableCell>
                <TableCell><b>${o.totalAmount?.toFixed(2)}</b></TableCell>
                <TableCell>{o.jurisdictions?.join(', ')}</TableCell>
              </TableRow>
            ))}
          </TableBody>
        </Table>
      </Paper>
    </Box>
  );
}