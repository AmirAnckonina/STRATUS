import PropTypes from 'prop-types';
import { ArrowUpIcon, ArrowDownIcon } from '@heroicons/react/24/solid';
import XIcon from '@heroicons/react/24/solid/MinusSmallIcon'
import { Button, Paper, Table, TableBody, TableCell, TableContainer, TableHead, TableRow, Typography, useTheme } from '@mui/material';
import ShoppingBagIcon from '@heroicons/react/24/solid/ShoppingBagIcon';
import { Avatar, Box, Card, CardContent, Divider, Stack, SvgIcon } from '@mui/material';

// ... Rest of the code remains unchanged



export const CustomMachineCard = (props) => {
  const { machine } = props;
  const theme = useTheme();

  const handleBuyNowClick = () => {
    window.location.href = 'https://aws.amazon.com/ec2/pricing/on-demand/';
  };

  // Dummy data for the Difference column (values from -100 to 100)
  const differenceData = [
    { fieldName: 'Type', value: machine.type, difference: 5 },
    { fieldName: 'Operating System', value: machine.specifications.operatingSystem.toString(), difference: -10 },
    { fieldName: 'Memory', value: machine.specifications.memory.asString.toString(), difference: 15 },
    { fieldName: 'Storage', value: machine.specifications.storage.asString.toString(), difference: -20 },
    { fieldName: 'Price', value: machine.specifications.price.priceAsString, difference: 25 },
  ];

  // Function to determine arrow color based on value
  const getArrowColor = (value, index) => {
    return index === 0 || index === 1 ? theme.palette.error.main : value < 0 ? theme.palette.error.main : theme.palette.success.main;
  };

  return (
    <Card
      sx={{
        display: 'flex',
        flexDirection: 'column',
        height: '100%'
      }}
    >
      <CardContent>
        <Box
          sx={{
            display: 'flex',
            justifyContent: 'center',
            pb: 3
          }}
        >
          <Avatar
            src={'/assets/logos/aws-logo.jpeg'}
            variant="square"
            sx={{ width: 125, height: 100 }}
          />
        </Box>
        <Typography
          align="center"
          gutterBottom
          variant="h5"
        >
          Description: 
        </Typography>        
      </CardContent>
      <Box sx={{ flexGrow: 1 }} />
      <Divider />
      <TableContainer component={Paper} sx={{ mt: 2 }}>
        <Table>
          <TableHead>
            <TableRow>
              <TableCell>Field Name</TableCell>
              <TableCell>Previos</TableCell>
              <TableCell>Alternative</TableCell>
              <TableCell>Difference</TableCell>
            </TableRow>
          </TableHead>
          <TableBody>
            {differenceData.map((data, index) => (
              <TableRow key={data.fieldName}>
                <TableCell>{data.fieldName}</TableCell>
                <TableCell>{data.value}</TableCell>
                <TableCell>{data.value}</TableCell>
                <TableCell>
                  <Box sx={{ display: 'flex', alignItems: 'center', color: getArrowColor(data.difference, index) }}>
                    {index === 0 || index === 1 ? <XIcon/> : data.difference > 0 ? <ArrowUpIcon fontSize="small" /> : <ArrowDownIcon fontSize="small"/>}
                    
                      {index === 0 || index === 1 ? null : `${Math.abs(data.difference)}%`}                    
                  </Box>
                </TableCell>
              </TableRow>
            ))}
          </TableBody>
        </Table>
      </TableContainer>
      <Divider />
      <Stack
        alignItems="center"
        direction="row"
        justifyContent="space-between"
        spacing={2}
        sx={{ p: 2 }}
      >
         <Stack
          alignItems="center"
          direction="row"
          spacing={1}
        >
          <Typography
            color="text.secondary"
            display="inline"
            variant="body2"
          >
            You can save up to 50% in month !
          </Typography>
        </Stack>
        <Stack
          alignItems="center"
          direction="row"
          spacing={1}
        >
          <SvgIcon
            color="action"
            fontSize="small"
          >
            <ShoppingBagIcon />
          </SvgIcon>
          <Button variant="outlined" size="small" onClick={handleBuyNowClick}>
           Buy Now
          </Button>
        </Stack>
      </Stack>
    </Card>
  );
};

CustomMachineCard.propTypes = {
  machine: PropTypes.object.isRequired
};
